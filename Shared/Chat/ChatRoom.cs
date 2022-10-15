using System;
using System.Collections.Generic;

namespace Netrunner.Shared.Chat
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Guid> Members { get; set; }
        public ICollection<Guid> Invitations { get; set; }
    }
}