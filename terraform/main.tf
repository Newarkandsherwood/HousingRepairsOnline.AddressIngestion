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

resource "azurerm_windows_function_app" "ingest-tenant-addresses-production" {
  name                = "hro-tenant-address-ingestion-production"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-tenant-addresses-production.name
    "BlobPath"           = var.tenant-csv-blob-path-production
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_windows_function_app" "ingest-tenant-addresses-staging" {
  name                = "hro-tenant-address-ingestion-staging"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-tenant-addresses-staging.name
    "BlobPath"           = var.tenant-csv-blob-path-staging
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_cosmosdb_sql_database" "hro-addresses" {
  name                = "housing-repairs-online-addresses"
  resource_group_name = var.resource-group
  account_name        = var.cosmos-account-name
}

resource "azurerm_cosmosdb_sql_container" "hro-tenant-addresses-production" {
  name                  = "tenant-production"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = azurerm_cosmosdb_sql_database.hro-addresses.name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}

resource "azurerm_cosmosdb_sql_container" "hro-tenant-addresses-staging" {
  name                  = "tenant-staging"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = azurerm_cosmosdb_sql_database.hro-addresses.name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}
