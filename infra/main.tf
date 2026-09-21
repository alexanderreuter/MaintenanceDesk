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
# The tenant and the object id of whoever runs Terraform, for Key Vault and role assignments.
data "azurerm_client_config" "current" {}
