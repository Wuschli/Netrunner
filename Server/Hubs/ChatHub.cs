using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Netrunner.Server.Models;
using Netrunner.Shared;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly IMongoCollection<ChatMessage> _chatMessages;

        public ChatHub(IDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _chatMessages = database.GetCollection<ChatMessage>(settings.ChatRoomCollectionName);
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _chatMessages.InsertOneAsync(message);
            await Clients.All.ReceiveMessage(message);
        }
    }
}