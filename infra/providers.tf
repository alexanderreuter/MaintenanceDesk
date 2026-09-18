terraform {
  required_version = ">= 1.16, < 2.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 5.6"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.9"
    }
  }

  backend "azurerm" {
    storage_account_name = "stmaintdesktfstate"
    container_name       = "tfstate"
    key                  = "maintenancedesk.tfstate"
    use_azuread_auth     = true
  }
}

provider "azurerm" {
  features {}

  resource_providers_to_register = [
    "Microsoft.App",
    "Microsoft.ContainerRegistry",
    "Microsoft.Insights",
    "Microsoft.KeyVault",
    "Microsoft.ManagedIdentity",
    "Microsoft.OperationalInsights",
    "Microsoft.ServiceBus",
    "Microsoft.Sql",
  ]
}