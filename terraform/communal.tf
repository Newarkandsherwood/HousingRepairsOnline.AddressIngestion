resource "azurerm_windows_function_app" "ingest-communal-addresses-production" {
  name                = "hro-communal-address-ingestion-production"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-communal-addresses-production.name
    "BlobPath"           = var.communal-csv-blob-path-production
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_windows_function_app" "ingest-communal-addresses-staging" {
  name                = "hro-communal-address-ingestion-staging"
  resource_group_name = var.resource-group
  location            = var.location

  storage_account_name       = var.storage-account
  storage_account_access_key = var.storage-account-primary-access-key
  service_plan_id            = azurerm_service_plan.ingest-addresses.id

  site_config {}

  app_settings = {
    "CosmosDBConnection" = "AccountEndpoint=${data.azurerm_cosmosdb_account.hro.endpoint};AccountKey=${data.azurerm_cosmosdb_account.hro.primary_key};"
    "DatabaseName"       = azurerm_cosmosdb_sql_database.hro-addresses.name
    "CollectionName"     = azurerm_cosmosdb_sql_container.hro-communal-addresses-staging.name
    "BlobPath"           = var.communal-csv-blob-path-staging
    "PartitionKey"       = var.partition-key
    "HousingProvider"    = var.housing-provider
  }
}

resource "azurerm_cosmosdb_sql_container" "hro-communal-addresses-production" {
  name                  = "communal-production"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = azurerm_cosmosdb_sql_database.hro-addresses.name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}

resource "azurerm_cosmosdb_sql_container" "hro-communal-addresses-staging" {
  name                  = "communal-staging"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = azurerm_cosmosdb_sql_database.hro-addresses.name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}
