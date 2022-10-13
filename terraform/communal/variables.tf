variable "location" {
  type        = string
  description = "Azure region to deploy module to"
}

variable "resource-group" {
  type        = string
  description = "Resource group name"
}

variable "storage-account" {
  type        = string
  description = "Storage account name"
}

variable "storage-account-primary-access-key" {
  type        = string
  description = "Storage account primary access key"
}

variable "communal-csv-blob-path-production" {
  type        = string
  description = "Path to production communal address data CSV"
}

variable "communal-csv-blob-path-staging" {
  type        = string
  description = "Path to staging communal address data CSV"
}

variable "partition-key" {
  type        = string
  description = "Partition key"
}

variable "housing-provider" {
  type        = string
  description = "Name of housing provider"
}

variable "cosmos-account-name" {
  type        = string
  description = "Name of CosmosDB account"
}

variable "address-type" {
  type        = string
  description = "Type of address data"
}
