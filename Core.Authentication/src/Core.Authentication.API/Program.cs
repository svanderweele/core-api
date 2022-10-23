using Amazon.DynamoDBv2;
using Core.Authentication.API.Contracts.Requests;
using Core.Authentication.API.Providers.Authentication;
using Core.Authentication.API.Repositories;
using Core.Authentication.API.Services;
using Core.Authentication.API.Settings;
using Core.Authentication.API.Validation;
using FluentValidation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthentication(CoreAuthHandler.SchemeName)
    .AddScheme<CoreAuthSchemeOptions, CoreAuthHandler>(CoreAuthHandler.SchemeName,null);


builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.KeyName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.KeyName));


var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();


builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IJwtService, JwtService>();

//Validation Services
builder.Services.AddTransient<IValidator<CreateCustomerRequest>, CreateCustomerRequestValidator>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//TODO: Only show swagger on non production environment
//TODO: Improve how we store server configuration

app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();