using System.Security.Claims;
using Guid = System.Guid;

namespace Netrunner.Server.Helpers;

public static class UserExtensions
{
    public static Guid GetId(this ClaimsPrincipal user)
    {
        var idString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idString == null)
            return Guid.Empty;
        return Guid.Parse(idString);
    }
}