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

module "tenant_address_ingestion" {
  source = "../shared"
  
  address-type = "tenant"
  location = var.location
  resource-group = var.resource-group
  storage-account = var.storage-account
  storage-account-primary-access-key = var.storage-account-primary-access-key
  csv-blob-path-production = var.tenant-csv-blob-path-production
  csv-blob-path-staging = var.tenant-csv-blob-path-staging
  partition-key = var.partition-key
  housing-provider = var.housing-provider
  cosmos-account-name = var.cosmos-account-name
}