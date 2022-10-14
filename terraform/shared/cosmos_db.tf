data "azurerm_cosmosdb_account" "hro" {
  name                = var.cosmos-account-name
  resource_group_name = var.resource-group
}

resource "azurerm_cosmosdb_sql_container" "hro-addresses-production" {
  name                  = "${var.address-type}-production"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = var.database-name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}

resource "azurerm_cosmosdb_sql_container" "hro-addresses-staging" {
  name                  = "${var.address-type}-staging"
  account_name          = var.cosmos-account-name
  resource_group_name   = var.resource-group
  database_name         = var.database-name
  partition_key_path    = var.partition-key
  partition_key_version = 1
  throughput            = 400
}
