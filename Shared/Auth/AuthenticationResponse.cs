namespace Netrunner.Shared.Auth
{
    public class AuthenticationResponse
    {
        public string UserName { get; init; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public bool Successful { get; set; }
        public string Error { get; set; }
    }
}