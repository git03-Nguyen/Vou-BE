using FluentValidation;
using MediatR;

namespace Shared.Validation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationErrors = new List<ValidationError>();
            
            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (validationResult.IsValid) continue;
                var errors = validationResult.Errors.Select(x => new ValidationError 
                {
                    Field = x.PropertyName,
                    Message = x.ErrorMessage
                });
                validationErrors.AddRange(errors);
            }

            if (validationErrors.Count == 0) return await next();
            var validationResultModel = new ValidationBaseResponse
            {
                Data = validationErrors
            };
            throw new ValidationException(validationResultModel);
        }
    }
}
