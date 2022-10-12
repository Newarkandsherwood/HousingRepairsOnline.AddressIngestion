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

data "azurerm_cosmosdb_account" "hro" {
  name                = var.cosmos-account-name
  resource_group_name = var.resource-group
}

resource "azurerm_service_plan" "ingest-addresses" {
  name                = "ingest-addresses-function-service-plan"
  resource_group_name = var.resource-group
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_cosmosdb_sql_database" "hro-addresses" {
  name                = "housing-repairs-online-addresses"
  resource_group_name = var.resource-group
  account_name        = var.cosmos-account-name
}