using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace Core.Gaming.CDK
{
    public class CoreGamingStack : Stack
    {
        internal CoreGamingStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here

            var dockerCode = DockerImageCode.FromImageAsset("src/Core.Gaming.API");

            
            //TODO: One gateway created in Terraform and shared
            var gateway = new RestApi(this, "gaming-auth-gateway", new RestApiProps()
            {

            });

            var testFunction = new Amazon.CDK.AWS.Lambda.DockerImageFunction(this, "gaming-image",
                new DockerImageFunctionProps()
                {
                    Code = dockerCode,
                    Description = "Testing a Docker function", Architecture = Architecture.ARM_64
                });

            gateway.Root.AddProxy(new ProxyResourceOptions()
            {
                DefaultIntegration = new LambdaIntegration(testFunction)
            });

        }
    }
}
