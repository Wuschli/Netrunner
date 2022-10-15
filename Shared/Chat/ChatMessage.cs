using System;

namespace Netrunner.Shared.Chat
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        public Guid SenderId { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}