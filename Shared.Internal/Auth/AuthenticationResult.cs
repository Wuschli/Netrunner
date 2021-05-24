using Newtonsoft.Json;

namespace Netrunner.Shared.Internal.Auth
{
    public class AuthenticationResult
    {
        [JsonProperty("Realm")]
        public string Realm { get; set; }

        [JsonProperty("AuthId")]
        public string AuthId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("extra")]
        public AuthenticationExtras Extra { get; set; }
    }
}