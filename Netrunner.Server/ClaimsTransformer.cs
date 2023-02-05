using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Netrunner.Server.Helpers;

public class ClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        principal.TransformRoleClaims();

        return Task.FromResult(principal);
    }
}