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

module "tenant_address_ingestion" {
  source = "./shared"

  address-type                       = "tenant"
  location                           = var.location
  resource-group                     = var.resource-group
  storage-account                    = var.storage-account
  storage-account-primary-access-key = var.storage-account-primary-access-key
  csv-blob-path-production           = var.tenant-csv-blob-path-production
  csv-blob-path-staging              = var.tenant-csv-blob-path-staging
  partition-key                      = var.partition-key
  housing-provider                   = var.housing-provider
  cosmos-account-name                = var.cosmos-account-name
  database-name                      = azurerm_cosmosdb_sql_database.hro-addresses.name
  service-plan-id                    = azurerm_service_plan.ingest-addresses.id
}

# module "communal_address_ingestion" {
#   source = "./shared"

#   address-type = "communal"
#   location = var.location
#   resource-group = var.resource-group
#   storage-account = var.storage-account
#   storage-account-primary-access-key = var.storage-account-primary-access-key
#   csv-blob-path-production = var.communal-csv-blob-path-production
#   csv-blob-path-staging = var.communal-csv-blob-path-staging
#   partition-key = var.partition-key
#   housing-provider = var.housing-provider
#   cosmos-account-name = var.cosmos-account-name
#   database-name = azurerm_cosmosdb_sql_database.hro-addresses.name
#   service-plan-id = azurerm_service_plan.ingest-addresses.id
# }