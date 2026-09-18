locals {
  location = "swedencentral"

  tags = {
    project     = "maintenancedesk"
    environment = "dev"
    managed-by  = "terraform"
  }
}

resource "azurerm_resource_group" "main" {
  name     = "rg-maintenancedesk-dev"
  location = local.location
  tags     = local.tags
}