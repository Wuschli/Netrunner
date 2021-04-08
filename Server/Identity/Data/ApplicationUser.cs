using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using Netrunner.Shared.User;

namespace Netrunner.Server.Identity.Data
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public List<string>? Rooms { get; set; }
        public List<string>? Invitations { get; set; }
        public List<Contact>? Contacts { get; set; }

        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }
    }
}