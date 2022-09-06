terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.0.0"
    }
  }
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
  skip_provider_registration = true
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

  site_config {
  }
}

data "azurerm_subscription" "primary" {
}

data "azurerm_storage_account" "example" {
  name                = var.storage-account
  resource_group_name = var.resource-group
}

resource "azurerm_role_assignment" "ingest_addresses_contributor" {
  scope                = data.azurerm_subscription.primary.id
  role_definition_name = "Contributor"
  principal_id         = azurerm_windows_function_app.example.identity[0].principal_id
}

# Allow our function's managed identity to have r/w access to the storage account
resource "azurerm_role_assignment" "ingest_addresses" {
  principal_id         = azurerm_windows_function_app.example.identity[0].principal_id
  scope                = data.azurerm_storage_account.example.id
  role_definition_name = "Storage Blob Data Reader"
}
