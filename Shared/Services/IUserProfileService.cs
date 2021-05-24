using System.Threading.Tasks;
using Netrunner.Shared.Users;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IUserProfileService
    {
        [WampProcedure("netrunner.user_profiles.get_profile")]
        Task<UserProfile?> GetUserProfile(string username);
    }
}