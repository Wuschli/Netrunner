using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            if (userName != null)
            {
                var user = _userManager.Users.Single(r => r.UserName == userName);
                if (user?.Rooms != null)
                {
                    foreach (var roomId in user.Rooms)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"room{roomId}");
                    }
                }
            }

            await base.OnConnectedAsync();
        }
    }
}