using System.ComponentModel.DataAnnotations;

namespace Netrunner.Shared.Identity
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string UserName { get; init; }
        public string Token { get; init; }
    }
}