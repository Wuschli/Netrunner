using MongoDB.Driver;

namespace Netrunner.Auth
{
    public class Config
    {
        public JwtConfig Jwt { get; set; }
        public DatabaseConfig Database { get; set; }
    }

    public class JwtConfig
    {
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }

    public class DatabaseConfig
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? RolesCollectionName { get; set; }
    }
}