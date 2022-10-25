output "subnets_public" {
  description = "A public subnet"
  value       = module.vpc.public_subnets
}
output "subnets_private" {
  description = "A private subnet"
  value       = module.vpc.private_subnets
}


output "vpc" {
  description = "The vpc"
  value       = module.vpc
}
