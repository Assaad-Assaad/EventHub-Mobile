using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Email { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
