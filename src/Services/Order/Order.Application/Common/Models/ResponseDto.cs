
using Newtonsoft.Json;

namespace Order.Application.Common.Models
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object? Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }

        public static ResponseDto Success(string message = "", object? result = null)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = message,
                Result = result
            };
        }

        public static ResponseDto Fail(string message = "", IDictionary<string, string[]>? errors = null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        public static T? DeserializeResult<T>(ResponseDto? responseDto)
        {
            if(responseDto is null || responseDto.IsSuccess == false)
                return default;

            string resultString = Convert.ToString(responseDto.Result) ?? "";
            T? deserializedResult = JsonConvert.DeserializeObject<T>(resultString);

            return deserializedResult;
        }
    }
}