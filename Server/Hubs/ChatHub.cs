using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
    }
}