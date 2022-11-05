using System.Text;
using Amazon.DynamoDBv2;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Exceptions;
using Core.Gaming.API.Repositories;
using Core.Gaming.API.Services;
using Core.Gaming.API.Settings;
using Core.Gaming.API.Validation;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//Configuration Setup
builder.Configuration.AddSystemsManager(source =>
{
    source.Path = "/dev/coreapp";
    source.ReloadAfter = TimeSpan.FromMinutes(5);
    source.OnLoadException = context => { Console.WriteLine($"Error Loading Config: {context}"); };
});

builder.Configuration.AddJsonFile("appsettings.json");

if (builder.Environment.EnvironmentName == Environments.Development)
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}

//Authentication Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection(JwtSettings.KeyName).Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new
            SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
                (jwtSettings.Secret)),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateLifetime = true,
        ValidateAudience = false
    };
});


// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddSwaggerGen(c =>
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


//AWS Configuration Setup
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);

builder.Services.AddCors(options =>
{
    options.DefaultPolicyName = "Default";
    options.AddPolicy("Default", policyBuilder =>
        policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.KeyName));

builder.Services.AddAWSService<IAmazonDynamoDB>();

var redisEndpoint = builder.Configuration.GetValue<string>("redis:endpoint");
var multiplexer = ConnectionMultiplexer.Connect(redisEndpoint);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IGameCollectionRepository, GameCollectionRepository>();
builder.Services.AddSingleton<IGameCategoryRepository, GameCategoryRepository>();

builder.Services.AddSingleton<ICollectionService, CollectionService>();
builder.Services.AddSingleton<IGameService, GameService>();

//Validation Services
builder.Services.AddTransient<IValidator<CreateGameRequest>, CreateGameRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//TODO: Only show swagger on non production environment
//TODO: Improve how we store server configuration

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();