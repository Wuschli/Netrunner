using System.Security.Claims;
using Newtonsoft.Json;
using Guid = System.Guid;

namespace Netrunner.Server.Helpers;

public static class UserExtensions
{
    public static Guid GetId(this ClaimsPrincipal user)
    {
        var idString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idString == null)
            throw new HttpResponseException(StatusCodes.Status401Unauthorized);
        return Guid.Parse(idString);
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        user.TransformRoleClaims();
        return user.IsInRole("admin");
    }

    public static void TransformRoleClaims(this ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = (ClaimsIdentity)principal.Identity;

        // flatten realm_access because Microsoft identity model doesn't support nested claims
        // by map it to Microsoft identity model, because automatic JWT bearer token mapping already processed here
        if (claimsIdentity.IsAuthenticated && claimsIdentity.HasClaim((claim) => claim.Type == "realm_access"))
        {
            var realmAccessClaim = claimsIdentity.FindFirst((claim) => claim.Type == "realm_access");
            var realmAccessAsDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(realmAccessClaim.Value);
            if (realmAccessAsDict["roles"] != null)
            {
                foreach (var role in realmAccessAsDict["roles"])
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            claimsIdentity.RemoveClaim(realmAccessClaim);
        }
    }
}