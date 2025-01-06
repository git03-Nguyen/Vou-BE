using Microsoft.AspNetCore.Mvc;

namespace Shared.Response;

public class ErrorResponse<T> where T : class
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ObjectResult ToObjectResult()
    {
        return new ObjectResult(this) { StatusCode = Status };
    }
}
