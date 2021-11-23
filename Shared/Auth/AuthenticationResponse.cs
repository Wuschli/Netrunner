using System.Collections.Generic;

namespace Netrunner.Shared.Auth
{
    public class AuthenticationResponse
    {
        public string AuthenticationId { get; init; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public bool Successful { get; init; }
        public List<string> Roles { get; init; } = new();
        public string? Error { get; init; }
    }
}