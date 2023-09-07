
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Cart.API.Extensions
{
    public static class TokenAuthenticationServiceExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                string secret = jwtSettings.GetValue<string>("Secret");
                string validIssuer = jwtSettings.GetValue<string>("ValidIssuer");
                string validAudience = jwtSettings.GetValue<string>("ValidAudience");

                byte[] key = Encoding.ASCII.GetBytes(secret);
                var securityKey = new SymmetricSecurityKey(key);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = validAudience,
                    ValidIssuer = validIssuer,
                    IssuerSigningKey = securityKey
                };
            });

            return services;
        }
    }
}