using System;

namespace Netrunner.Shared.Chat;

public record ChatMessage
{
    public Guid Id { get; set; }

    public Guid RoomId { get; init; }

    public Guid SenderId { get; set; }

    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
}