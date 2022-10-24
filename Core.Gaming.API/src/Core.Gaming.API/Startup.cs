using Amazon.DynamoDBv2;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Providers.Authentication;
using Core.Gaming.API.Repositories;
using Core.Gaming.API.Services;
using Core.Gaming.API.Settings;
using Core.Gaming.API.Validation;
using FluentValidation;

namespace Core.Gaming.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddAuthentication(CoreAuthHandler.SchemeName)
            .AddScheme<CoreAuthSchemeOptions, CoreAuthHandler>(CoreAuthHandler.SchemeName,null);

        
        services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
        
        var awsOptions = Configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonDynamoDB>();

        services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.KeyName));
        
        services.AddSingleton<IGameRepository, GameRepository>();
        services.AddSingleton<IGameCollectionRepository, GameCollectionRepository>();
        services.AddSingleton<IGameCategoryRepository, GameCategoryRepository>();

        services.AddSingleton<ICollectionService, CollectionService>();
        
        //Validation Services
        services.AddTransient<IValidator<CreateGameRequest>, CreateGameRequestValidator>();

        
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/",
                async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
        });
    }
}