
using Catalog.API.Extensions;
using Catalog.API.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace Catalog.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string message = GetMessage(ex);
            var errors = GetErrors(ex);

            var responseDto = ResponseDto.Fail(message, errors);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(responseDto));
        }

        public string GetMessage(Exception ex)
        {
            string message = ex switch
            {
                ValidationException => "One or more validation error occurred.",
                _ => ex.Message
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