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


