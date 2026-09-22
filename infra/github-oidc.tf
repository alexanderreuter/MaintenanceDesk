resource "azurerm_role_assignment" "github_acr_push" {
  count                = var.github_principal_id == null ? 0 : 1
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPush"
  principal_id         = var.github_principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "github_api_deploy" {
  count                = var.github_principal_id == null ? 0 : 1
  scope                = azurerm_container_app.api.id
  role_definition_name = "Container Apps Contributor"
  principal_id         = var.github_principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "github_worker_deploy" {
  count                = var.github_principal_id == null ? 0 : 1
  scope                = azurerm_container_app.worker.id
  role_definition_name = "Container Apps Contributor"
  principal_id         = var.github_principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "github_secrets_user" {
  count                = var.github_principal_id == null ? 0 : 1
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.github_principal_id
  principal_type       = "ServicePrincipal"
}
