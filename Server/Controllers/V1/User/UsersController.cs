using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Netrunner.Server.Services;
using Netrunner.Server.Services.Internal;
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

        public async Task<UserProfile> GetUserProfile(string username)
        {
            throw new NotImplementedException();
            //var user = await _userManager.FindByNameAsync(username);
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

            //if (string.IsNullOrWhiteSpace(contact.AuthenticationId))
            //    return NotFound();

            //if (user.AuthenticationId.Equals(contact.AuthenticationId))
            //    return Conflict("Cannot add self as contact");

            //if (user.Contacts != null && user.Contacts.Any(c => c.AuthenticationId.Equals(contact.AuthenticationId)))
            //    return Conflict("Contact already added");

            //var other = await _userManager.FindByNameAsync(contact.AuthenticationId);
            //if (other == null)
            //    return NotFound();

            //user.Contacts ??= new List<Contact>();
            //user.Contacts.Add(contact);

            //var identityResult = await _userManager.UpdateAsync(user);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }

        public async Task RemoveContact(string username)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //if (string.IsNullOrWhiteSpace(username) || user.Contacts == null)
            //    return NotFound();

            //var removed = user.Contacts.RemoveAll(c => c.AuthenticationId.Equals(username));

            //if (removed == 0)
            //    return NotFound();

            //var identityResult = await _userManager.UpdateAsync(user);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }
    }
}