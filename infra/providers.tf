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
    # For Azure features azurerm has not implemented, currently only the Azure SQL free offer.
    azapi = {
      source  = "azure/azapi"
      version = "~> 2.12"
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
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }

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
