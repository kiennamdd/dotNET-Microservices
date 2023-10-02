
using Newtonsoft.Json;

namespace Cart.API.Models
{
    public class ResponseDto<TResult>
    {
        public bool IsSuccess { get; set; } = true;
        public TResult? Result { get; set; } = default;
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
    }

    public class ResponseDto: ResponseDto<object>
    {
        public static ResponseDto Success(string message = "", object? result = default)
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

        public TCast? DeserializeResult<TCast>()
        {
            string strResult = Convert.ToString(Result) ?? "";
            TCast? result = JsonConvert.DeserializeObject<TCast>(strResult);

            return result;
        }

        public ResponseDto<TResult> ToResponseDtoWithCastedResult<TResult>()
        {
            return new ResponseDto<TResult>
            {
                IsSuccess = IsSuccess,
                Message = Message,
                Result = Result != null ? DeserializeResult<TResult>() : default,
                Errors = Errors
            };
        }
    }
}