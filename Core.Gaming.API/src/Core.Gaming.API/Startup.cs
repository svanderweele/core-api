using Amazon.DynamoDBv2;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Providers.Authentication;
using Core.Gaming.API.Repositories;
using Core.Gaming.API.Services;
using Core.Gaming.API.Settings;
using Core.Gaming.API.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

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
            .AddScheme<CoreAuthSchemeOptions, CoreAuthHandler>(CoreAuthHandler.SchemeName, null);


        services.AddAWSLambdaHosting(LambdaEventSource.RestApi);


        var awsOptions = Configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonDynamoDB>();

        services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.KeyName));
        services.Configure<JwtSettings>(Configuration.GetSection(JwtSettings.KeyName));

        services.AddSingleton<IJwtService, JwtService>();

        services.AddSingleton<IGameRepository, GameRepository>();
        services.AddSingleton<IGameCollectionRepository, GameCollectionRepository>();
        services.AddSingleton<IGameCategoryRepository, GameCategoryRepository>();

        services.AddSingleton<ICollectionService, CollectionService>();
        services.AddSingleton<IGameService, GameService>();

        var redisEndpoint = Configuration.GetSection("Redis").GetValue<string>("Url");
        var multiplexer = ConnectionMultiplexer.Connect(redisEndpoint);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        

        //Validation Services
        services.AddTransient<IValidator<CreateGameRequest>, CreateGameRequestValidator>();


        
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization", Type = SecuritySchemeType.ApiKey
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


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