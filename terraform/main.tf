terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.0.0"
    }
  }
}

provider "azurerm" {
  features {}
  skip_provider_registration = true
}

data "archive_file" "ingest_addresses" {
  type        = "zip"
  output_path = "IngestAddresses.zip"
  source_dir  = "../HousingRepairsOnline.AddressIngestion"
}

resource "azurerm_storage_blob" "ingest_addresses" {

  name                   = "${data.archive_file.ingest_addresses.output_base64sha256}.zip"
  storage_account_name   = var.storage-account
  storage_container_name = var.storage-container-name
  type                   = "Block"
  source                 = data.archive_file.ingest_addresses.output_path
  content_md5            = data.archive_file.ingest_addresses.output_md5
}


resource "azurerm_service_plan" "example" {
  name                = "example-app-service-plan"
  resource_group_name = var.resource-group
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "example" {
  name                = "test-function-hro-maysa"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.example.id

  identity {
    type = "SystemAssigned"
  }

  site_config {
  }
  app_settings = {
    WEBSITE_RUN_FROM_PACKAGE = "https://${var.storage-account}.blob.core.windows.net/${var.storage-container-name}/${urlencode(azurerm_storage_blob.ingest_addresses.name)}",
  }
}

data "azurerm_subscription" "primary" {
}

data "azurerm_storage_account" "example" {
  name                = var.storage-account
  resource_group_name = var.resource-group
}

# Allow our function's managed identity to have r/w access to the storage account
resource "azurerm_role_assignment" "ingest_addresses" {
  principal_id         = azurerm_windows_function_app.example.identity[0].principal_id
  scope                = data.azurerm_storage_account.example.id
  role_definition_name = "Storage Blob Data Reader"
}
