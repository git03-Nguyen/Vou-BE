using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Response;
using Shared.Validation;

namespace Shared.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = HttpStatusCode.InternalServerError.ToInt();
        var response = new BaseResponse<object>
        {
            Status = httpContext.Response.StatusCode,
            Message = "Internal Server Error"
        };

        switch (exception)
        {
            case ValidationException:
                httpContext.Response.StatusCode = HttpStatusCode.BadRequest.ToInt();
                response.Status = httpContext.Response.StatusCode;
                response.Message = "Validation failed";
                response.Data = (exception as ValidationException)?.ValidationErrors;
                break;
            case NotSupportedException:
                httpContext.Response.StatusCode = HttpStatusCode.NotImplemented.ToInt();
                break;
            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = HttpStatusCode.Unauthorized.ToInt();
                break;
            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        var result = JsonSerializer.Serialize(response, _jsonSerializerOptions);
        await httpContext.Response.WriteAsync(result, cancellationToken);
        return true;
    }
}