
using FluentValidation;
using Identity.API.Models;
using Newtonsoft.Json;
using Identity.API.Extensions;

namespace Identity.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;    
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                if(ex is not ValidationException)
                {
                    _logger.LogError(ex, "Unhandled error occurred.");
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string message = GetErrorMessage(ex);
            var errors = GetErrors(ex);

            var failResponse = ResponseDto.Fail(message, errors);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(failResponse));
        }

        public string GetErrorMessage(Exception ex)
        {
            string message = ex switch {
                ValidationException => "One or more validation exception occurred.",
                _ => "Internal service error."
            };

            return message;
        }

        public IDictionary<string, string[]>? GetErrors(Exception ex)
        {
            if(ex is ValidationException exception)
            {
                return exception.GetValidationErrors();
            }

            return null;
        }
    }
}