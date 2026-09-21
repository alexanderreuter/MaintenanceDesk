resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-maintenancedesk"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "PerGB2018"
  retention_in_days   = 30

  # A runaway log loop can't run up a bill; ingestion stops until the next day instead.
  daily_quota_gb = 1

  tags = local.tags
}

resource "azurerm_application_insights" "main" {
  name                = "appi-maintenancedesk"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  application_type    = "web"

  # Workspace-based: telemetry lands in the same workspace as the container logs.
  workspace_id = azurerm_log_analytics_workspace.main.id

  retention_in_days    = 30
  daily_data_cap_in_gb = 1

  tags = local.tags
}
