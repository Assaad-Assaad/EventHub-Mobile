using System.ComponentModel.DataAnnotations;

namespace EventHub.Api.Entities
{
    public class UserEvent
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }

        public bool IsFavorite { get; set; } 
        public bool IsSignedIn { get; set; }
    }
}
