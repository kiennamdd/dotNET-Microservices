using Discount.API.Extensions;
using Discount.API.Models;
using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace Discount.API.Middlewares
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

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var responseDto = ResponseDto.Fail(message: GetMessage(e), errors: GetErrors(e));

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(responseDto));
        }

        private string GetMessage(Exception e)
        {
            string message = e switch
            {
                ValidationException => "One or more validation exception occurred.",
                _ => e.Message
            };

            return message;
        }

        private IDictionary<string, string[]>? GetErrors(Exception e)
        {
            if(e.GetType() != typeof(ValidationException))
                return null;

            return ((ValidationException)e).GetValidationErrors();
        }
    }
}
