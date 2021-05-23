using System.Text.Json.Serialization;

namespace Netrunner.Shared.Auth
{
    public class AuthenticationResult
    {
        [JsonIgnore]
        //[JsonPropertyName("realm")]
        public string Realm { get; set; }

        [JsonPropertyName("authid")]
        public string AuthId { get; set; }

        [JsonPropertyName("authrole")]
        public string Role { get; set; }

        [JsonIgnore]
        public AuthenticationExtras Extra { get; set; }
    }

    public class AuthenticationExtras
    {
    }

    public class AuthenticationDetails
    {
        public long Session { get; set; }
        public string Ticket { get; set; }
    }
}