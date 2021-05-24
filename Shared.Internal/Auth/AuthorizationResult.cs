using Newtonsoft.Json;

namespace Netrunner.Shared.Internal.Auth
{
    public class AuthorizationResult
    {
        [JsonProperty("allow")]
        public bool Allow { get; set; }

        [JsonProperty("disclose")]
        public bool Disclose { get; set; }

        [JsonProperty("cache")]
        public bool Cache { get; set; }
    }
}