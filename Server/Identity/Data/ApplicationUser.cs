using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Netrunner.Server.Identity.Data
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public ICollection<string>? Rooms { get; set; }
        public ICollection<string>? Invitations { get; set; }

        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }
    }
}