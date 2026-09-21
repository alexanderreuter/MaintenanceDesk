output "registry_login_server" {
  description = "Prefix for image names pushed to the registry."
  value       = azurerm_container_registry.main.login_server
}
