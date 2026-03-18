namespace Shared.Validation;

public class ValidationException : Exception
{
    public List<ValidationError> ValidationErrors { get; }

    public ValidationException(List<ValidationError> validationErrors)
        : base(BuildMessage(validationErrors))
    {
        ValidationErrors = validationErrors;
    }

    private static string BuildMessage(IEnumerable<ValidationError> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (errors.Count == 0)
        {
            return "Validation failed.";
        }

        return $"Validation failed: {string.Join("; ", errors.Select(x => string.IsNullOrWhiteSpace(x.Field) ? x.Message : $"{x.Field}: {x.Message}"))}";
    }
}