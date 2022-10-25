module "dynamodb_table" {
  source = "terraform-aws-modules/dynamodb-table/aws"

  name     = "${terraform.workspace}-${var.table-name}"
  hash_key = var.hashKey
  attributes = var.attributes
}
