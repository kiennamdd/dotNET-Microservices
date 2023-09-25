
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Order.API.Middlewares
{
    public class HttpClientHandlingAuthentication: DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientHandlingAuthentication(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;    
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization;
            var token2 = _httpContextAccessor.HttpContext?.GetTokenAsync("access_token").GetAwaiter().GetResult();

            if(token2 is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token2);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}