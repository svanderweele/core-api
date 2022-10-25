variable "table-name" {
  type        = string
  description = "The name used for the Table"
}

variable "hashKey" {
  type        = string
  description = "The hash key used for the Table"
}


variable "attributes" {
  type = list(object({
    name : string
    type : string
  }))
}
