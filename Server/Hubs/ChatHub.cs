using System.Threading.Tasks;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Hubs
{
    //public class ChatHub : Hub<IChatHub>
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;

    //    public ChatHub(UserManager<ApplicationUser> userManager)
    //    {
    //        _userManager = userManager;
    //    }

    //    public override async Task OnConnectedAsync()
    //    {
    //        var username = Context.User?.Identity?.Name;
    //        if (username != null)
    //        {
    //            var user = _userManager.Users.Single(r => r.Username == username);
    //            if (user?.Rooms != null)
    //            {
    //                foreach (var roomId in user.Rooms)
    //                {
    //                    await Groups.AddToGroupAsync(Context.ConnectionId, $"room{roomId}");
    //                }
    //            }
    //        }

    //        await base.OnConnectedAsync();
    //    }
    //}
}