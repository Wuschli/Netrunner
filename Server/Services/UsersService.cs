using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Netrunner.Server.Attributes;
using Netrunner.Server.Services.Internal;
using Netrunner.Shared.Services;
using Netrunner.Shared.Users;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Server.Services
{
    [WampService]
    public class UsersService : IContactService, IUserProfileService
    {
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public UsersService(IUserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfile?> GetUserProfile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new WampException("netrunner.error.not_found");
            var profile = _mapper.Map<UserProfile>(user);
            return profile;
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");
            return user.Contacts ?? Enumerable.Empty<Contact>();
        }

        public async Task AddContact(Contact contact)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            if (string.IsNullOrWhiteSpace(contact.Username))
                throw new WampException("netrunner.error.not_found");

            if (user.Username.Equals(contact.Username))
                throw new WampException("netrunner.error.operation_failed");

            if (user.Contacts != null && user.Contacts.Any(c => c.Username.Equals(contact.Username)))
                throw new WampException("netrunner.error.operation_failed");

            var other = await _userManager.FindByNameAsync(contact.Username);
            if (other == null)
                throw new WampException("netrunner.error.not_found");

            user.Contacts ??= new List<Contact>();
            user.Contacts.Add(contact);

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }

        public async Task RemoveContact(string username)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            if (string.IsNullOrWhiteSpace(username) || user.Contacts == null)
                throw new WampException("netrunner.error.not_found");

            var removed = user.Contacts.RemoveAll(c => c.Username.Equals(username));

            if (removed == 0)
                throw new WampException("netrunner.error.not_found");

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }
    }
}