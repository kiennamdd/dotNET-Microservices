
using FluentValidation;
using Newtonsoft.Json;
using Order.API.Extensions;
using Order.Application.Common.Models;
using Order.Domain.Exceptions;

namespace Order.API.Middlewares
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
            catch(Exception e)
            {
                if(e is not ValidationException)
                {
                    _logger.LogError(e, "Unhandled exception occurred.");
                }

                await HandleExceptionAsync(context, e);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string message = GetMessage(ex);
            var errors = GetErrors(ex);

            var responseDto = ResponseDto.Fail(message, errors);

            context.Response.StatusCode = GetStatusCode(ex);
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(responseDto));
        }

        public int GetStatusCode(Exception ex)
        {
            int statusCode = ex switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                OrderDomainException => StatusCodes.Status400BadRequest,
                UnableToChangeOrderStatusException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return statusCode;
        }

        public string GetMessage(Exception ex)
        {
            string message = ex switch
            {
                ValidationException => "One or more validation error occurred.",
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