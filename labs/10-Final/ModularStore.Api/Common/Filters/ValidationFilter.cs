using FluentValidation;

namespace ModularStore.Api.Common.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.Arguments.FirstOrDefault(x => x is T) as T;
        if (arg == null) return Results.BadRequest("Invalid request body.");

        var validationResult = await _validator.ValidateAsync(arg);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}
