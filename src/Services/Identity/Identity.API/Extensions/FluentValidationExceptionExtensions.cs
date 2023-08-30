using FluentValidation;

namespace Identity.API.Extensions
{
    public static class FluentValidationExceptionExtensions
    {
        public static IDictionary<string, string[]> GetValidationErrors(this ValidationException exception)
        {
            return exception.Errors.GroupBy(o => o.PropertyName, o => o.ErrorMessage)
                                    .ToDictionary(o => o.Key, o=>o.ToArray());
        }
    }
}