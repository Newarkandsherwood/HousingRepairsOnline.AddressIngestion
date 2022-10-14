resource "azurerm_windows_function_app" "ingest-addresses-production" {
  name                = "hro-${var.address-type}-address-ingestion"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = var.service-plan-id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = var.database-name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-addresses-production.name
    "BlobPath"           = var.csv-blob-path-production
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_windows_function_app_slot" "ingest-addresses-staging" {
  name                       = "${var.address-type}-staging"
  function_app_id            = azurerm_windows_function_app.ingest-addresses-production.id
  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = var.database-name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-addresses-staging.name
    "BlobPath"           = var.csv-blob-path-staging
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}
