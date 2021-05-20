using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Services;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Controllers.V1.User
{
    public class UsersController
    {
        private readonly IUserManager _userService;

        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public UsersController(IUserManager userService, IUserManager userManager, IMapper mapper)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfile> GetUserProfile(string userName)
        {
            throw new NotImplementedException();
            //var user = await _userManager.FindByNameAsync(userName);
            //if (user == null)
            //    return NotFound();
            //var profile = _mapper.Map<UserProfile>(user);
            //return Ok(profile);
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();
            //return Ok(user.Contacts ?? Enumerable.Empty<Contact>());
        }

        public async Task AddContact(Contact contact)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //if (string.IsNullOrWhiteSpace(contact.Username))
            //    return NotFound();

            //if (user.Username.Equals(contact.Username))
            //    return Conflict("Cannot add self as contact");

            //if (user.Contacts != null && user.Contacts.Any(c => c.Username.Equals(contact.Username)))
            //    return Conflict("Contact already added");

            //var other = await _userManager.FindByNameAsync(contact.Username);
            //if (other == null)
            //    return NotFound();

            //user.Contacts ??= new List<Contact>();
            //user.Contacts.Add(contact);

            //var identityResult = await _userManager.UpdateAsync(user);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }

        public async Task RemoveContact(string userName)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //if (string.IsNullOrWhiteSpace(userName) || user.Contacts == null)
            //    return NotFound();

            //var removed = user.Contacts.RemoveAll(c => c.Username.Equals(userName));

            //if (removed == 0)
            //    return NotFound();

            //var identityResult = await _userManager.UpdateAsync(user);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }
    }
}