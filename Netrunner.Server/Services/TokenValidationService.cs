using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Netrunner.Server.Helpers;

namespace Netrunner.Server.Services;

public interface ITokenValidationService
{
    Task<ClaimsPrincipal?> ValidateAsync(string token);
}

public class TokenValidationService : ITokenValidationService
{
    private readonly JwtBearerOptions _options;

    public TokenValidationService(IConfiguration config)
    {
        _options = new JwtBearerOptions
        {
            Authority = config["Jwt:Authority"],
            ClaimsIssuer = config["Jwt:Authority"],
            Audience = config["Jwt:Audience"]
        };
    }

    public async Task<ClaimsPrincipal?> ValidateAsync(string token)
    {
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{_options.Authority}/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        var config = await configurationManager.GetConfigurationAsync();


        var parameters = new TokenValidationParameters
        {
            ValidIssuer = _options.ClaimsIssuer,

            ValidAudience = _options.Audience,
            IssuerSigningKeys = config.SigningKeys
        };
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        try
        {
            var user = handler.ValidateToken(token, parameters, out _);
            return user;
        }
        catch
        {
            return null;
        }
    }
}