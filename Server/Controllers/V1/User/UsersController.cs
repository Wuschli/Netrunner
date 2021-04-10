using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Services;
using Netrunner.Shared.User;

namespace Netrunner.Server.Controllers.V1.User
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound();
            var profile = _mapper.Map<UserProfile>(user);
            return Ok(profile);
        }

        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();
            return Ok(user.Contacts ?? Enumerable.Empty<Contact>());
        }

        [HttpPost("contacts")]
        public async Task<ActionResult> AddContact([FromBody] Contact contact)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();

            if (string.IsNullOrWhiteSpace(contact.UserName))
                return NotFound();

            if (user.UserName.Equals(contact.UserName))
                return Conflict("Cannot add self as contact");

            if (user.Contacts != null && user.Contacts.Any(c => c.UserName.Equals(contact.UserName)))
                return Conflict("Contact already added");

            var other = await _userManager.FindByNameAsync(contact.UserName);
            if (other == null)
                return NotFound();

            user.Contacts ??= new List<Contact>();
            user.Contacts.Add(contact);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }

        [HttpDelete("contacts/{userName}")]
        public async Task<ActionResult> RemoveContact(string userName)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();

            if (string.IsNullOrWhiteSpace(userName) || user.Contacts == null)
                return NotFound();

            var removed = user.Contacts.RemoveAll(c => c.UserName.Equals(userName));

            if (removed == 0)
                return NotFound();

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError, "Could add contact to user");
        }
    }
}