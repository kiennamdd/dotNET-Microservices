

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Cart.API.Middlewares
{
    public class AuthenticationHttpClientHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAssessor;

        public AuthenticationHttpClientHandler(IHttpContextAccessor contextAssessor)
        {
            _contextAssessor = contextAssessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _contextAssessor.HttpContext?.Request.Headers.Authorization;

            if(token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}