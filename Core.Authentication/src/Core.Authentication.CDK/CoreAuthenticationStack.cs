using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace CoreAuthenticationCdk
{
    public class CoreAuthenticationStack : Stack
    {
        internal CoreAuthenticationStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here

            var s3Policy = new Amazon.CDK.AWS.IAM.PolicyStatement(new Amazon.CDK.AWS.IAM.PolicyStatementProps
            {
                Actions = new string[] { "s3:*" },
                Resources = new string[] { "*" },
            });

            var dynamoDbPolicy = new Amazon.CDK.AWS.IAM.PolicyStatement(new Amazon.CDK.AWS.IAM.PolicyStatementProps
            {
                Actions = new string[] { "dynamodb:*" },
                Resources = new string[] { "*" },
            });

            var ssmPolicy = new Amazon.CDK.AWS.IAM.PolicyStatement(new Amazon.CDK.AWS.IAM.PolicyStatementProps
            {
                Actions = new string[] { "ssm:GetParametersByPath" },
                Resources = new string[] { "*" },
            });


            var dockerCode = DockerImageCode.FromImageAsset("./src/Core.Authentication.API");
            var apiFunction = new Amazon.CDK.AWS.Lambda.DockerImageFunction(this, "authentication",
                new DockerImageFunctionProps()
                {
                    Code = dockerCode,
                    Description = "Testing a Docker function",
                    //TODO: Check if local, if so add architecture flag
                    // Architecture = Architecture.ARM_64,
                    InitialPolicy = new PolicyStatement[]
                    {
                        s3Policy,
                        dynamoDbPolicy,
                        ssmPolicy
                    }
                });


            //TODO: One gateway created in Terraform and shared
            var gateway = new RestApi(this, "core-auth-gateway", new RestApiProps()
            {
            });


            gateway.Root.AddProxy(new ProxyResourceOptions()
            {
                DefaultIntegration = new LambdaIntegration(apiFunction),
            });
        }
    }
}