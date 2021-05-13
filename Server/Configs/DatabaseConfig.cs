namespace Netrunner.Server.Configs
{
    public class DatabaseConfig
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? RolesCollectionName { get; set; }
        public string? ChatRoomCollectionName { get; set; }
        public string? ChatMessageCollectionName { get; set; }
    }
}