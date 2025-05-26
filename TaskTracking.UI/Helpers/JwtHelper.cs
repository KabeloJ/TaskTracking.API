using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskTracking.UI.Models;

public static class JwtHelper
{
    public static ClaimsPrincipal? GetClaimsPrincipal(string jwtToken, IConfigurationSection? JwtSettings)
    {

        
        string? secretKey =  JwtSettings?.GetSection("SecretKey").Value;
        string? Issuer = JwtSettings?.GetSection("Issuer").Value;
        string? Audience = JwtSettings?.GetSection("Audience").Value;

        if (string.IsNullOrEmpty(jwtToken)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
        }
        catch
        {
            return null; // Invalid or expired token
        }
    }

    public static string? GetUserRole(ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.Role)?.Value;
    }
    public static UserClaimsModel? GetUserClaims(ClaimsPrincipal? principal)
    {
        var user = new UserClaimsModel()
        {
            UserName = principal?.FindFirst(ClaimTypes.Name)?.Value,
            Role = principal?.FindFirst(ClaimTypes.Role)?.Value
        };
        return user;
    }

}