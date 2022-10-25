using Core.Gaming.API.Contracts.Responses;
using FluentValidation;

namespace Core.Gaming.API.Validation;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _request;

    public ExceptionMiddleware(RequestDelegate request)
    {
        _request = request;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = 400;
            var messages = exception.Errors.Select(x => x.ErrorMessage).ToList();
            var validationFailureResponse = new ValidationErrorResponse()
            {
                Errors = messages
            };
            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
        catch (Exception exception)
        {
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            context.Response.StatusCode = 500;
            //TODO: Consider using Newtonsoft package to serialize Exception class instead of using ToString()
            var response = new ErrorResponse(exception.Message, isDevelopment ? exception.ToString() : null);
            await context.Response.WriteAsJsonAsync(response);

        }
    }
}