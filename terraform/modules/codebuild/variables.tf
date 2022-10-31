variable "codebuild-bucket-name" {
  description = "Name of the bucket that will contain codebuild cache"
}

variable "project-name" {
  description = "Name of the bucket that will contain codebuild cache"
}


variable "repo-url" {
  description = "Name of the bucket that will contain codebuild cache"
}

variable "githubPersonalAccessToken" {
  type        = string
  description = "Personal Access Token for GitHub access"
}

variable "jwtSecret" {
  type        = string
  description = "JWT Secret used for Authentication"
}

