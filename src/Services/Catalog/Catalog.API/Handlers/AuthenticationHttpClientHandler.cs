using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;

namespace Catalog.API.Handlers
{
    public class AuthenticationHttpClientHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;   

        public AuthenticationHttpClientHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = _accessor.HttpContext?.Request.Headers.Authorization;

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}