using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Netrunner.Server.Models;
using Netrunner.Shared;

namespace Netrunner.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatMessage> _chatMessages;

        public ChatHub(IDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _chatMessages = database.GetCollection<ChatMessage>(settings.ChatCollectionName);
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _chatMessages.InsertOneAsync(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}