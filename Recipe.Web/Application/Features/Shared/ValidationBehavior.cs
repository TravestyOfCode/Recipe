using System.Linq;

namespace Recipe.Web.Application.Features.Shared;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var validationContext = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

            var errors = results.SelectMany(e => e.Errors).Where(e => e != null);

            if (errors.Any())
            {
                var result = Activator.CreateInstance(typeof(TResponse), 400) as IResult;

                foreach (var error in errors)
                {
                    result.AddError(new Error(error.PropertyName, error.ErrorMessage));
                }

                return (TResponse)result;
            }
        }

        return await next();
    }
}
