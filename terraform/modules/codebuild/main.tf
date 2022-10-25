module "private_s3_bucket" {
  source = "../bucket"
  acl    = "private"

  bucket_name = var.codebuild-bucket-name

  website = {
    # conflicts with "error_document"
    #        redirect_all_requests_to = {
    #          host_name = "https://modules.tf"
    #        }

    index_document = "index.html"
    error_document = "error.html"
    routing_rules = [{
      condition = {
        http_error_code_returned_equals = 404
      },
      redirect = {
        protocol           = "https"
        http_redirect_code = 302
        replace_key_with   = "index.html"
      }
    }]
  }
}

resource "aws_iam_role" "example" {
  name = "${terraform.workspace}-codebuild-deployment-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow",
        Principal = {
          Service : "codebuild.amazonaws.com"
        },
        Action = ["sts:AssumeRole"]
      },
    ]
  })

  #   assume_role_policy = <<EOF
  # {
  #   "Version": "2012-10-17",
  #   "Statement": [
  #     {
  #       "Effect": "Allow",
  #       "Principal": {
  #         "Service": "codebuild.amazonaws.com"
  #       },
  #       "Action": ["sts:AssumeRole"]
  #     }
  #   ]
  # }
  # EOF

  managed_policy_arns = [aws_iam_policy.codebuild_policy.arn]


}



resource "aws_iam_policy" "codebuild_policy" {
  name = "${terraform.workspace}-codebuild-policy"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      # TODO: Tighten up the codebuild policy
      {
        Action   = ["ec2:Describe*"]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action   = ["cloudformation:**"]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action   = ["s3:**"]
        Effect   = "Allow"
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy" "example" {
  role = aws_iam_role.example.name

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      # TODO: Tighten up the codebuild policy
      {
        Action   = ["logs:CreateLogGroup", "logs:CreateLogStream", "logs:PutLogEvents"]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action = [
          "ssm:GetParameter"
        ]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action = [
          "ec2:CreateNetworkInterface",
          "ec2:DescribeDhcpOptions",
          "ec2:DescribeNetworkInterfaces",
          "ec2:DeleteNetworkInterface",
          "ec2:DescribeSubnets",
          "ec2:DescribeSecurityGroups",
          "ec2:DescribeVpcs"
        ]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action   = ["ec2:Describe*"]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action   = ["cloudformation:**"]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action = ["s3:**"]
        Effect = "Allow"
        Resource = [
          "${module.private_s3_bucket.bucket_arn}",
          "${module.private_s3_bucket.bucket_arn}/*"
        ]
      },
      {
        Action   = ["iam:PassRole"]
        Effect   = "Allow"
        Resource = "*"
      },

    ]
  })
}



resource "aws_codebuild_source_credential" "example" {
  auth_type   = "PERSONAL_ACCESS_TOKEN"
  server_type = "GITHUB"
  token       = var.githubPersonalAccessToken
}

resource "aws_codebuild_project" "example" {
  name                   = "${terraform.workspace}-${var.project-name}-project"
  build_timeout          = 5
  service_role           = aws_iam_role.example.arn
  concurrent_build_limit = 1

  # TODO: Update artifacts type
  artifacts {
    location       = module.private_s3_bucket.bucket_id
    name           = var.project-name
    namespace_type = "NONE"
    packaging      = "NONE"
    type           = "S3"
  }


  cache {
    type     = "S3"
    location = module.private_s3_bucket.bucket_id
  }

  environment {
    compute_type                = "BUILD_GENERAL1_SMALL"
    image                       = "aws/codebuild/standard:5.0"
    type                        = "LINUX_CONTAINER"
    image_pull_credentials_type = "CODEBUILD"

    environment_variable {
      name  = "APP_ENVIRONMENT"
      value = terraform.workspace == "dev" ? "dev" : "prod"
    }
  }


  logs_config {
    cloudwatch_logs {
      group_name  = "log-group"
      stream_name = "log-stream"
    }

    s3_logs {
      status   = "ENABLED"
      location = "${module.private_s3_bucket.bucket_id}/build-log"
    }
  }

  source {
    type            = "GITHUB"
    location        = var.repo-url
    git_clone_depth = 1
    buildspec       = terraform.workspace == "dev" ? "buildspec-dev.yml" : "buildspec.yml"
  }


  source_version = terraform.workspace == "dev" ? "develop" : "main"

  tags = {
    Name = "Valhalla BE Codebuild"
  }
}


resource "aws_codebuild_webhook" "example" {
  project_name = aws_codebuild_project.example.name
  build_type   = "BUILD"

  filter_group {
    filter {
      type    = "EVENT"
      pattern = "PUSH"
    }

    filter {
      type    = "HEAD_REF"
      pattern = terraform.workspace == "dev" ? "develop" : "main"
    }
  }
}
