resource "azurerm_key_vault" "main" {
  name                = "kv-maintenancedesk"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"

  rbac_authorization_enabled = true

  soft_delete_retention_days = 7
  purge_protection_enabled   = false

  tags = local.tags
}

resource "azurerm_role_assignment" "terraform_secrets_officer" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets Officer"
  principal_id         = data.azurerm_client_config.current.object_id
  principal_type       = "User"
}

resource "azurerm_role_assignment" "api_secrets_user" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.api.principal_id
  principal_type       = "ServicePrincipal"
}

locals {
  # '--' is how .NET configuration spells ':' in a key, so this binds to ConnectionStrings:MaintenanceDesk.
  sql_connection_string = join(";", [
    "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433",
    "Initial Catalog=${azapi_resource.database.name}",
    "User ID=${azurerm_mssql_server.main.administrator_login}",
    "Password=${random_password.sql_admin.result}",
    "Encrypt=True",
    "TrustServerCertificate=False",

    "Connection Timeout=60",
  ])
}

resource "azurerm_key_vault_secret" "sql_connection_string" {
  name         = "ConnectionStrings--MaintenanceDesk"
  key_vault_id = azurerm_key_vault.main.id
  value        = local.sql_connection_string

  depends_on = [azurerm_role_assignment.terraform_secrets_officer]
}
