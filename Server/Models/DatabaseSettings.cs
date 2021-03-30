namespace Netrunner.Server.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; }
        public string RolesCollectionName { get; set; }
        public string ChatRoomCollectionName { get; set; }
        public string ChatMessageCollectionName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string UsersCollectionName { get; set; }
        string RolesCollectionName { get; set; }
        string ChatRoomCollectionName { get; set; }
        string ChatMessageCollectionName { get; set; }
    }
}