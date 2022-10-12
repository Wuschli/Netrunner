using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Netrunner.Shared.Auth
{
    public class TokenPayload
    {
        [JsonPropertyName("name")]
        public string? Username { get; set; }

        [JsonPropertyName("identifier")]
        public string? UserId { get; set; }

        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }

        [JsonPropertyName("exp")]
        public long? Expiration { get; set; }

        [JsonPropertyName("iat")]
        public long? IssuedAt { get; set; }

        [JsonPropertyName("sub")]
        public string? Subject { get; set; }

        [JsonPropertyName("iss")]
        public string? Issuer { get; set; }

        [JsonPropertyName("aud")]
        public string? Audience { get; set; }

        [JsonPropertyName("nbf")]
        public string? NotBefore { get; set; }

        [JsonPropertyName("jti")]
        public string? TokenId { get; set; }

        [JsonPropertyName("sid")]
        public string? SessionId { get; set; }
    }
}