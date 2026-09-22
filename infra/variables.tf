variable "client_ip_address" {
  description = "Public IP allowed through the SQL firewall, for running migrations from a workstation. Leave unset elsewhere."
  type        = string
  default     = null
}

variable "github_principal_id" {
  description = "Object id of the maintenancedesk-github service principal, created by hand outside Terraform. az ad sp show --id <appId> --query id -o tsv"
  type        = string
  default     = null
}
