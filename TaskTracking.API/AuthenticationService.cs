using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskTracking.API
{
    public static class AuthenticationService
    {
        public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var key = Encoding.UTF8.GetBytes(jwtSettings.GetSection("SecretKey").Value);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        ValidAudience = jwtSettings.GetSection("Audience").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
        }
    }
}
