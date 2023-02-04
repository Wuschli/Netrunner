namespace Netrunner.Shared.Chat;

public record VideoRoomAccess
{
    public string Url { get; init; }
    public string Token { get; init; }
}