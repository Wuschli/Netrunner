using System.ComponentModel.DataAnnotations;

namespace Netrunner.Shared.Identity
{
    public class RegistrationRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegistrationResponse
    {
        public string UserName { get; init; }

        public string Token { get; init; }
    }
}