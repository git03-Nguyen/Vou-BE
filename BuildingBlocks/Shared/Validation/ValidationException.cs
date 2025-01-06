namespace Shared.Validation;

public class ValidationException : Exception
{
    public ValidationErrorResponse ValidationErrorResponse { get; }
    
    public ValidationException(ValidationErrorResponse validationErrorResponse) : base(validationErrorResponse.ToString())
    {
        ValidationErrorResponse = validationErrorResponse;
    }
    
}