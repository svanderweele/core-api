output "bucket_arn" {
  description = "The ARN of the bucket."
  value       = module.s3_bucket.s3_bucket_arn
}


output "bucket_id" {
  description = "The Id of the bucket."
  value       = module.s3_bucket.s3_bucket_id
}

