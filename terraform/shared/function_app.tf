resource "azurerm_service_plan" "ingest-addresses" {
  name                = "ingest-addresses-function-service-plan"
  resource_group_name = var.resource-group
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "ingest-addresses-production" {
  name                = "hro-${var.address-type}-address-ingestion-production"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-addresses-production.name
    "BlobPath"           = var.csv-blob-path-production
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_windows_function_app" "ingest-addresses-staging" {
  name                = "hro-${var.address-type}-address-ingestion-staging"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-addresses-staging.name
    "BlobPath"           = var.csv-blob-path-staging
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}
