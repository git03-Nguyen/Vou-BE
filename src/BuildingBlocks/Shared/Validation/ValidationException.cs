namespace Shared.Validation;

public class ValidationException : Exception
{
    public List<ValidationError> ValidationErrors { get; }
    public ValidationException(List<ValidationError> validationErrors) : base(validationErrors.ToString())
    {
        ValidationErrors = validationErrors;
    }
    
}