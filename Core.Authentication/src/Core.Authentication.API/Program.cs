using Amazon.DynamoDBv2;
using Core.Authentication.API.Contracts.Requests;
using Core.Authentication.API.Repositories;
using Core.Authentication.API.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddSwaggerGen();
builder.Services.AddAWSService<IAmazonDynamoDB>();

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>(provider =>
    new CustomerRepository(provider.GetRequiredService<IAmazonDynamoDB>(),
        builder.Configuration.GetValue<string>("Database:TableName")));


builder.Services.AddTransient<IValidator<CreateCustomerRequest>, CreateCustomerRequestValidator>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();