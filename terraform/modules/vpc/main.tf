module "vpc" {
  source = "terraform-aws-modules/vpc/aws"

  name = "${terraform.workspace}-valhalla-vpc"
  cidr = "10.0.0.0/16"

  azs             = ["eu-west-1a", "eu-west-1b", "eu-west-1c"]
  private_subnets = ["10.0.1.0/24", "10.0.2.0/24"]
  public_subnets  = ["10.0.101.0/24"]

  enable_nat_gateway = false
  enable_vpn_gateway = false


  default_network_acl_egress = [
    {
      protocol   = -1
      rule_no    = 100
      action     = "allow"
      cidr_block = "0.0.0.0/0"
      from_port  = 0
      to_port    = 0
    }
  ]

  default_network_acl_ingress = [
    {
      protocol   = -1
      rule_no    = 100
      action     = "allow"
      cidr_block = "0.0.0.0/0"
      from_port  = 0
      to_port    = 0
    }
  ]

}


# resource "aws_subnet" "private_1" {
#   vpc_id            = module.vpc.vpc_id
#   cidr_block        = "10.0.1.0/24"
#   availability_zone = "eu-west-1a"


#   tags = {
#     Name = "Private Main 1"
#   }
# }
# resource "aws_subnet" "private_2" {
#   vpc_id            = module.vpc.vpc_id
#   cidr_block        = "10.0.101.0/24"
#   availability_zone = "eu-west-1b"

#   tags = {
#     Name = "Private Main 2"
#   }
# }

# resource "aws_subnet" "public_1" {
#   vpc_id                  = module.vpc.vpc_id
#   cidr_block              = "10.0.3.0/24"
#   availability_zone       = "eu-west-1c"
#   map_public_ip_on_launch = true

#   tags = {
#     Name = "Public Main 1",
#   }
# }



# resource "aws_network_acl" "public_access_acl" {
#   vpc_id = module.vpc.vpc_id

#   ingress {
#     protocol   = -1
#     rule_no    = 100
#     action     = "allow"
#     cidr_block = "0.0.0.0/0"
#     from_port  = 0
#     to_port    = 0
#   }

#   egress {
#     protocol   = -1
#     rule_no    = 100
#     action     = "allow"
#     cidr_block = "0.0.0.0/0"
#     from_port  = 0
#     to_port    = 0
#   }
# }

# resource "aws_network_acl_association" "public_acl_association" {
#   network_acl_id = aws_network_acl.public_access_acl.id
#   subnet_id      = aws_subnet.public_1.id
# }


# resource "aws_internet_gateway" "gw" {
#   vpc_id = module.vpc.vpc_id
# }
