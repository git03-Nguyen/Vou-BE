namespace Shared.Validation;

public class ValidationException : Exception
{
    public ValidationBaseResponse ValidationBaseResponse { get; }
    
    public ValidationException(ValidationBaseResponse validationBaseResponse) : base(validationBaseResponse.ToString())
    {
        ValidationBaseResponse = validationBaseResponse;
    }
    
}