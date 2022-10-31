using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SES.Actions;
using Constructs;

namespace Core.Gaming.CDK
{
    public class CoreGamingStack : Stack
    {
        internal CoreGamingStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here

            var vpcPolicy = new Amazon.CDK.AWS.IAM.PolicyStatement(new Amazon.CDK.AWS.IAM.PolicyStatementProps
            {
                Actions = new string[]
                {
                    "ec2:DescribeNetworkInterfaces",
                    "ec2:CreateNetworkInterface",
                    "ec2:DeleteNetworkInterface",
                    "ec2:DescribeInstances",
                    "ec2:AttachNetworkInterface"
                },
                Resources = new string[] { "*" },
            });


            var dynamoDbPolicy = new Amazon.CDK.AWS.IAM.PolicyStatement(new Amazon.CDK.AWS.IAM.PolicyStatementProps
            {
                Actions = new string[] { "dynamodb:*" },
                Resources = new string[] { "*" },
            });

            
            //TODO: One gateway created in Terraform and shared
            var gateway = new RestApi(this, "gaming-auth-gateway", new RestApiProps()
            {
            });

            var vpc = Amazon.CDK.AWS.EC2.Vpc.FromLookup(this, "vpc", new VpcLookupOptions()
            {
                VpcId = "vpc-06d9475282a5e5587"
            });

            var securityGroup = Amazon.CDK.AWS.EC2.SecurityGroup.FromLookupById(this, "sg", "sg-042eeb1b4be1b89e1");


            var dockerImage = Code.FromAssetImage("./src/Core.Gaming.API");
            var testFunction = new Amazon.CDK.AWS.Lambda.Function(this, "gaming-image",
                new FunctionProps()
                {
                    Code = dockerImage,
                    Runtime = Runtime.FROM_IMAGE,
                    Architecture = Architecture.ARM_64,
                    Description = "Testing a Docker function",
                    InitialPolicy = new[] { vpcPolicy, dynamoDbPolicy },
                    Vpc = vpc,
                    SecurityGroups = new[] { securityGroup },
                    Timeout = Duration.Seconds(30)
                });

            gateway.Root.AddProxy(new ProxyResourceOptions()
            {
                DefaultIntegration = new LambdaIntegration(testFunction)
            });
        }
    }
}