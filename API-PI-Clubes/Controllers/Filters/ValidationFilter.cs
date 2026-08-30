using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using AppValidationException = API_PI_Clubes.Application.Exceptions.ValidationException;

namespace API_PI_Clubes.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null) continue;

                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                    continue;

                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);

                if (result.IsValid) continue;

                foreach (var failure in result.Errors)
                {
                    errors[failure.PropertyName] = errors.TryGetValue(failure.PropertyName, out var existing)
                        ? existing.Append(failure.ErrorMessage).ToArray()
                        : new[] { failure.ErrorMessage };
                }
            }

            if (errors.Count > 0)
            {
                throw new AppValidationException(
                    "Um ou mais erros de validação ocorreram.",
                    errors);
            }


            await next();
        }
    }
}