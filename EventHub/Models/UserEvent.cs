

namespace EventHub.Models
{
    public class UserEvent
    {
        [Indexed]
        public int UserId { get; set; }

        [Indexed]
        public int EventId { get; set; }

        [Ignore]
        public User? User { get; set; }

        [Ignore]
        public Event? Event { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsSignedIn { get; set; }

        [Indexed]
        public bool IsSynced { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
