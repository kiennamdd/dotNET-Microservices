using FluentValidation;
using FluentValidation.Results;

namespace Discount.API.Extensions
{
    public static class FluentValidationExceptionExtensions
    {
        public static IDictionary<string, string[]> GetValidationErrors(this ValidationException result)
        {
            return result.Errors
                .GroupBy(o => o.PropertyName, o => o.ErrorMessage)
                .ToDictionary(o => o.Key, o => o.ToArray());
        }
    }
}
