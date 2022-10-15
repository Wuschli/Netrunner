using System;

namespace Netrunner.Shared.Chat;

public record LeaveChatRoom
{
    public Guid RoomId { get; set; }
}