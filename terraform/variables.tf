variable "project" {
  type        = string
  description = "Housing Repairs Online"
}

variable "cosmos-connection-string" {
  type        = string
  description = "cosmos donnection string"
  sensitive   = true
}
variable "database-name" {
  type        = string
  description = "database name"
  sensitive   = true
}

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

variable "app-service-plan" {
  type        = string
  description = "App service plan name"
}

variable "storage-account-primary-access-key" {
  type        = string
  description = "storage account primary access key"
}

variable "storage-container-name" {
  type        = string
  description = "storage container name"
}
