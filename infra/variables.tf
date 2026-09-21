variable "client_ip_address" {
  description = "Public IP allowed through the SQL firewall, for running migrations from a workstation. Leave unset elsewhere."
  type        = string
  default     = null
}
