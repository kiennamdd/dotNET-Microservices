using FluentValidation;

namespace Catalog.API.Extensions
{
    public static class FluentValidationExceptionExtensions
    {
        public static IDictionary<string, string[]> GetValidationErrors(this ValidationException exception)
        {
            var dict = exception.Errors.GroupBy(o => o.PropertyName, o => o.ErrorMessage).ToDictionary(o => o.Key, o => o.ToArray());
            return dict;
        }
    }
}