terraform {
  backend "s3" {
    bucket = "terraform-bucket-svdw"
    key    = "terraform-key"
    region = "eu-west-1"
  }
}

provider "aws" {
  region = var.region

  default_tags {

    tags = {
      Terraform   = true
      Environment = terraform.workspace
    }
  }
}


# TODO: Store Terraform State Remotely

module "private_s3_bucket" {
  source      = "./modules/bucket"
  bucket_name = var.bucket_private_name
  acl         = "private"
}


module "vpc" {
  source = "./modules/vpc"
}


module "codebuild_be" {
  project-name              = "core-api-project"
  repo-url                  = "https://github.com/svanderweele/core-api.git"
  source                    = "./modules/codebuild"
  codebuild-bucket-name     = "codebuild-bucket-be"
  githubPersonalAccessToken = var.personalAccessToken
}


module "users-db" {
  source     = "./modules/database"
  table-name = "users"
  hashKey    = "email"
  attributes = [{
    name = "email"
    type = "S"
    },
  ]
}

module "games-db" {
  source     = "./modules/database"
  table-name = "games"
  hashKey    = "id"
  attributes = [{
    name = "id"
    type = "S"
    },
  ]
}

module "game-collections-db" {
  source     = "./modules/database"
  table-name = "game-collections"
  hashKey    = "id"
  attributes = [{
    name = "id"
    type = "S"
    },
  ]
}


module "game-categories-db" {
  source     = "./modules/database"
  table-name = "game-categories"
  hashKey    = "id"
  attributes = [{
    name = "id"
    type = "S"
    },
  ]
}



module "sqs-queue" {
  source     = "./modules/sqs"
  queue-name = "registration"
}

resource "aws_elasticache_cluster" "example" {
  cluster_id        = "${terraform.workspace}-redis-cluster"
  engine            = "redis"
  node_type         = "cache.t3.micro"
  num_cache_nodes   = 1
  engine_version    = "6.2"
  port              = 6379
  subnet_group_name = "${terraform.workspace}-subnet-group"

  depends_on = [
    aws_elasticache_subnet_group.subnet_group
  ]
}

resource "aws_elasticache_subnet_group" "subnet_group" {
  name       = "${terraform.workspace}-subnet-group"
  subnet_ids = [module.vpc.subnets_public[0]]
}
