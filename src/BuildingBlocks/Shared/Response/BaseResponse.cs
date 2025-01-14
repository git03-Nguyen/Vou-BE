using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;

namespace Shared.Response;

public class BaseResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    
    public BaseResponse ToInternalErrorResponse(string message = "Internal Server Error")
    {
        Status = HttpStatusCode.InternalServerError.ToInt();
        Message = message;
        return this;
    }
    
    public BaseResponse ToBadRequestResponse(string message = "Bad Request")
    {
        Status = HttpStatusCode.BadRequest.ToInt();
        Message = message;
        return this;
    }
    
    public BaseResponse ToNotFoundResponse(string message = "Not Found")
    {
        Status = HttpStatusCode.NotFound.ToInt();
        Message = message;
        return this;
    }
    
    public BaseResponse ToUnauthorizedResponse(string message = "Unauthorized")
    {
        Status = HttpStatusCode.Unauthorized.ToInt();
        Message = message;
        return this;
    }
    
    public BaseResponse ToUnConfirmedEmailResponse(string message = "Email is not confirmed")
    {
        Status = 450;
        Message = message;
        return this;
    }
    
    public BaseResponse ToForbiddenResponse(string message = "Forbidden")
    {
        Status = HttpStatusCode.Forbidden.ToInt();
        Message = message;
        return this;
    }

    public BaseResponse ToSuccessResponse()
    {
        Status = HttpStatusCode.OK.ToInt();
        return this;
    }
    
    public ObjectResult ToObjectResult()
    {
        return new ObjectResult(this) { StatusCode = Status };
    }
}

public class BaseResponse<T>: BaseResponse where T : class
{
    public T? Data { get; set; }
    
    public BaseResponse<T> ToSuccessResponse(T? data, string? message = null)
    {
        Status = HttpStatusCode.OK.ToInt();
        Message = message;
        Data = data;
        return this;
    }
}
