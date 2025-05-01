using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventHub.Api.Entities
{
    public class User
    {
        [Key]
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

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
