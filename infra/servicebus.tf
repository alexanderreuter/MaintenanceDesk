resource "azurerm_servicebus_namespace" "main" {
  name                = "sb-maintenancedesk"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"

  local_auth_enabled = false

  minimum_tls_version = "1.2"

  tags = local.tags
}

resource "azurerm_servicebus_queue" "events" {
  name         = "maintenance-request-events"
  namespace_id = azurerm_servicebus_namespace.main.id

  lock_duration      = "PT1M"
  max_delivery_count = 10

  default_message_ttl = "P14D"

  dead_lettering_on_message_expiration = true
}

resource "azurerm_role_assignment" "api_servicebus_sender" {
  scope                = azurerm_servicebus_queue.events.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azurerm_user_assigned_identity.api.principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "worker_servicebus_receiver" {
  scope                = azurerm_servicebus_queue.events.id
  role_definition_name = "Azure Service Bus Data Receiver"
  principal_id         = azurerm_user_assigned_identity.worker.principal_id
  principal_type       = "ServicePrincipal"
}
