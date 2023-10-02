using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using Cart.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;

namespace ASPNET_MVC.Services
{
    public class HttpService : IHttpService
    {
        private readonly ILogger<HttpService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public HttpService(ILogger<HttpService> logger, IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public HttpClient GetClient(string clientName, string baseUrl = "")
        {
            HttpClient client = _httpClientFactory.CreateClient(clientName);

            if(!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }

            return client;
        }

        public async Task<ResponseDto> SendAsync(RequestDto requestDto, HttpClient client)
        {
            try
            {
                // Prepare request message
                var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = requestDto.HttpMethod,
                    RequestUri = new Uri(client.BaseAddress?.OriginalString + requestDto.Url)
                };

                if(requestDto.IncludeAccessToken)
                {
                    string accessToken = _tokenProvider.GetAccessToken() ?? string.Empty;
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
                }

                switch(requestDto.ContentType)
                {
                    case ContentTypes.ApplicationJson:
                        httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        break;
                    default:
                        httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        break;
                }

                if(requestDto.Data != null)
                {
                    httpRequestMessage.Content = GetHttpContent(requestDto.Data, requestDto.ContentType);
                }

                // Send request message and receive response message
                HttpResponseMessage httpResponseMessage = await client.SendAsync(httpRequestMessage);

                var strContent = await httpResponseMessage.Content.ReadAsStringAsync();
                ResponseDto? responseDTO = JsonConvert.DeserializeObject<ResponseDto>(strContent);

                // Return result
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Server response fail status.\n\tClient Url: {client.BaseAddress}."
                                            + $"\n\tHttp Request Url: {httpRequestMessage.RequestUri}."
                                            + $"\n\tStatus code: {httpResponseMessage.StatusCode}");
                    return ResponseDto.Fail(responseDTO?.Message ?? httpResponseMessage.ReasonPhrase ?? "Internal server error.");
                }

                if(responseDTO is null)
                {
                    _logger.LogError($"Invalid response type from server.\n\tClient Url: {client.BaseAddress}."
                                            + $"\n\tHttp Request Url: {httpRequestMessage.RequestUri}.");
                    responseDTO = ResponseDto.Fail("Internal server error.");
                }

                return responseDTO;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unknown error while sending message.\n\tClient Url: {client.BaseAddress}.");
                return ResponseDto.Fail(ex.Message);
            }
        }

        private HttpContent GetHttpContent(object data, string contentType)
        {
            HttpContent content;

            if(contentType == ContentTypes.MultipartFormData)
            {
                var formContent = new MultipartFormDataContent();

                foreach(PropertyInfo propertyInfo in data.GetType().GetProperties())
                {
                    object? value = propertyInfo.GetValue(data);

                    if (value is IFormFile fileValue)
                    {
                        if(fileValue != null)
                        {
                            var streamContent = new StreamContent(fileValue.OpenReadStream());
                            formContent.Add(streamContent, propertyInfo.Name, fileValue.FileName);
                        }
                    }
                    else if (value is IEnumerable<IFormFile> files)
                    {
                        if (files != null)
                        {
                            foreach(IFormFile file in files)
                            {
                                var streamContent = new StreamContent(file.OpenReadStream());
                                formContent.Add(streamContent, propertyInfo.Name, file.FileName);
                            }
                        }
                    }
                    else
                    {
                        var strContent = new StringContent(Convert.ToString(value) ?? "");
                        formContent.Add(strContent, propertyInfo.Name);
                    }
                }

                content = formContent;
            }
            else
            {
                content = new StringContent(JsonConvert.SerializeObject(data) , Encoding.UTF8, "application/json");
            }

            return content;
        }
    }
}