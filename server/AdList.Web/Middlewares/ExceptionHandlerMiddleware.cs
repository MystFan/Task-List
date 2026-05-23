using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdList.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AdList.Web.Middlewares;

internal sealed class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException exception)
        {
            if (exception.InnerException != null)
            {
                _logger.LogError(exception, exception.Message);
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            await WriteResponseAsync(context, exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            await WriteResponseAsync(context, exception);
        }
    }

    private static Task WriteResponseAsync(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An error occured while processing the request.",
            Extensions =
            {
                {
                    "message", exception.ToString()
                }
            }
        };

        if (exception is DomainException domainServicesException)
        {
            if (domainServicesException.AdditionalData.ValidationErrors != null)
            {
                domainServicesException.AdditionalData.ValidationErrors = domainServicesException.AdditionalData.ValidationErrors
                    .Select(error => new
                    {
                        key = error.Key[..1].ToLowerInvariant() + error.Key[1..],
                        value = error.Value
                    }).ToDictionary(error => error.key, error => error.value);
            }

            problemDetails.Extensions.Add("additionalData", domainServicesException.AdditionalData);
            problemDetails.Extensions.Add("code", domainServicesException.ReasonCode.ToString());
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}