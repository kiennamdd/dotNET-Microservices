
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Catalog.API.Extensions
{
    public static class AddTokenAuthenticationExtensions
    {
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options => 
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                string secret = jwtSettings.GetValue<string>("Secret") ?? throw new ArgumentNullException("Secret");
                string validAudience = jwtSettings.GetValue<string>("ValidAudience") ?? throw new ArgumentNullException("ValidAudience");
                string validIssuer = jwtSettings.GetValue<string>("ValidIssuer") ?? throw new ArgumentNullException("ValidIssuer");

                byte[] key = Encoding.ASCII.GetBytes(secret);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            return services;
        }
    }
}