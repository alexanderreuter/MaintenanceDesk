output "registry_login_server" {
  description = "Prefix for image names pushed to the registry."
  value       = azurerm_container_registry.main.login_server
}

output "api_url" {
  description = "Public HTTPS address of the API."
  value       = "https://${azurerm_container_app.api.ingress[0].fqdn}"
}
