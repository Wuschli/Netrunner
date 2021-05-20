using System.ComponentModel.DataAnnotations;

namespace Netrunner.Shared.Auth
{
    public class RegistrationRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}