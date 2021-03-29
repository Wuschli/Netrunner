﻿using System.ComponentModel.DataAnnotations;

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
}