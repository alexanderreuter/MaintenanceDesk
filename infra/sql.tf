resource "random_password" "sql_admin" {
  length  = 32
  special = true

  # A ';' would end a connection string early and quotes complicate escaping.
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

resource "azurerm_mssql_server" "main" {
  name                          = "sql-maintenancedesk"
  resource_group_name           = azurerm_resource_group.main.name
  location                      = azurerm_resource_group.main.location
  version                       = "12.0"
  administrator_login           = "mdadmin"
  administrator_login_password  = random_password.sql_admin.result
  minimum_tls_version           = "1.2"
  public_network_access_enabled = true
  tags                          = local.tags
}

resource "azurerm_mssql_database" "main" {
  name                        = "sqldb-maintenancedesk"
  server_id                   = azurerm_mssql_server.main.id
  sku_name                    = "GP_S_Gen5_2"
  min_capacity                = 0.5
  auto_pause_delay_in_minutes = 60
  max_size_gb                 = 32
  storage_account_type        = "Local"
  collation                   = "SQL_Latin1_General_CP1_CI_AS"
  zone_redundant              = false
  tags                        = local.tags
}

# The free offer is not in azurerm yet, so patch it straight onto the resource.
resource "azapi_update_resource" "sql_free_limit" {
  type        = "Microsoft.Sql/servers/databases@2025-01-01"
  resource_id = azurerm_mssql_database.main.id

  body = {
    properties = {
      useFreeLimit                = true
      freeLimitExhaustionBehavior = "AutoPause"
    }
  }
}

# 0.0.0.0 to 0.0.0.0 is Azure's marker for "allow Azure services", not a real address range.
resource "azurerm_mssql_firewall_rule" "azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "client" {
  count            = var.client_ip_address == null ? 0 : 1
  name             = "ClientIP"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = var.client_ip_address
  end_ip_address   = var.client_ip_address
}
