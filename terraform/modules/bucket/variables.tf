variable "bucket_name" {
  description = "The name of the bucket to create."
}
variable "acl" {
  type = string
}

variable "website" {
  default = {}
}

