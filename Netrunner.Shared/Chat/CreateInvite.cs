using System;

namespace Netrunner.Shared.Chat;

public record CreateInvite
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
}