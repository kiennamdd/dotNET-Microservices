
using FluentValidation;

namespace Cart.API.Extensions
{
    public static class FluentValidationExceptionExtension
    {
        public static IDictionary<string, string[]> GetValidationErrors(this ValidationException exception)
        {
            return exception.Errors
                            .GroupBy(o => o.PropertyName, o=>o.ErrorMessage)
                            .ToDictionary(o => o.Key, o => o.ToArray());
        }
    }
}