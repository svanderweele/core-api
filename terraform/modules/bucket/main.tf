module "s3_bucket" {
  source = "terraform-aws-modules/s3-bucket/aws"

  bucket = "${terraform.workspace}-${var.bucket_name}"
  acl    = var.acl

  website = var.website

  force_destroy = true

  versioning = {
    enabled = true
  }
}
