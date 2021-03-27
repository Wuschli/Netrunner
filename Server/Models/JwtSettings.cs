namespace Netrunner.Server.Models
{
    public class JwtSettings : IJwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireDays { get; set; }
    }

    public interface IJwtSettings
    {
        string Key { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        int ExpireDays { get; set; }
    }
}