using System.Net;
using System.Text.Json;
using Shared.Extensions;
using Shared.Response;

namespace Shared.Validation;

public class ValidationErrorResponse : ErrorResponse<List<ValidationError>>
{
    public int Status { get; set; } = HttpStatusCode.BadRequest.ToInt();
    public string Message { get; set; } = "Validation failed";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}