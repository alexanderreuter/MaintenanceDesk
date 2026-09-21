resource "azurerm_container_app_environment" "main" {
  name                = "cae-maintenancedesk"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
  logs_destination           = "log-analytics"

  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }

  tags = local.tags
}

resource "azurerm_container_app" "api" {
  name                         = "ca-maintenancedesk-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id

  revision_mode = "Single"

  workload_profile_name = "Consumption"

  tags = local.tags

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.api.id]
  }

  registry {
    server   = azurerm_container_registry.main.login_server
    identity = azurerm_user_assigned_identity.api.id
  }

  ingress {
    external_enabled           = true
    target_port                = 8080
    transport                  = "auto"
    allow_insecure_connections = false

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  secret {
    name  = "appinsights-connection-string"
    value = azurerm_application_insights.main.connection_string
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name = "api"

      image = "mcr.microsoft.com/k8se/quickstart:latest"

      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "ASPNETCORE_FORWARDEDHEADERS_ENABLED"
        value = "true"
      }

      env {
        name  = "KeyVault__Uri"
        value = azurerm_key_vault.main.vault_uri
      }

      env {
        name  = "ServiceBus__Namespace"
        value = "${azurerm_servicebus_namespace.main.name}.servicebus.windows.net"
      }

      env {
        name  = "AZURE_CLIENT_ID"
        value = azurerm_user_assigned_identity.api.client_id
      }

      env {
        name        = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        secret_name = "appinsights-connection-string"
      }
    }
  }

  lifecycle {
    ignore_changes = [template[0].container[0].image]
  }

  depends_on = [azurerm_role_assignment.api_acr_pull]
}
