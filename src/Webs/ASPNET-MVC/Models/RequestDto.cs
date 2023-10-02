using System.Net.Mime;
using ASPNET_MVC.Constants;

namespace ASPNET_MVC.Models
{
    public class RequestDto
    {
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string ContentType { get; set; } = ContentTypes.ApplicationJson;
        public bool IncludeAccessToken { get; set; } = false;
    }
}