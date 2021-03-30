using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Netrunner.Server.Identity.Data
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }
    }
}