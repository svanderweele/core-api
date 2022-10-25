variable "region" {
  description = "The region of the provider to use."
}
variable "bucket_private_name" {
  description = "The private name of the bucket."
}

variable "personalAccessToken" {
  type      = string
  sensitive = true

  validation {
    condition     = substr(var.personalAccessToken, 0, 4) == "ghp_"
    error_message = "The Github Personal Token must has the wrong format. Must start with ghp"
  }
}
