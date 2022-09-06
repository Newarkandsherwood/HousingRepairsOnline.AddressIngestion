variable "project" {
    type = string
    description = "Housing Repairs Online"
}

variable "environment" {
    type = string
    description = "Environment (dev / stage / prod)"
}

variable "location" {
    type = string
    description = "Azure region to deploy module to"
}

variable "resource-group" {
    type = string
    description = "Resource group name"
}

variable "storage-account" {
    type = string
    description = "Storage account name"
}

variable "app-service-plan" {
    type = string
    description = "App service plan name"
}

variable "storage-account-primary-access-key" {
    type = string
    description = "storage account primary access key"
}

variable "storage-container-name" {
    type = string
    description = "storage container name"
}
