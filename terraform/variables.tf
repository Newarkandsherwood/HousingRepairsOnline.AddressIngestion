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

variable "storage-container-name" {
  type        = string
  description = "storage container name"
}

variable "tenant-csv-blob-path" {
  type        = string
  description = "Path to tenant address data CSV"
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