using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        [Ignore]
        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
