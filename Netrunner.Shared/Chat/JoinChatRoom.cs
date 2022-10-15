using System;

namespace Netrunner.Shared.Chat;

public record JoinChatRoom
{
    public Guid RoomId { get; set; }
}