using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Order.API.Extensions
{
    public static class AddTokenAuthenticationServiceExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => 
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                string secret = jwtSettings.GetValue<string>("Secret");
                string validAudience = jwtSettings.GetValue<string>("ValidAudience");
                string validIssuer = jwtSettings.GetValue<string>("ValidIssuer");

                byte[] key = Encoding.ASCII.GetBytes(secret);
                var securityKey = new SymmetricSecurityKey(key);

                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
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