using System.Text.Json.Serialization;

namespace Netrunner.Server.Identity
{
    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public RefreshToken? RefreshToken { get; set; }
    }
}