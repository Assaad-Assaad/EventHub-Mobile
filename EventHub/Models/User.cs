using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Email { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 6)]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";

        [Ignore]
        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
