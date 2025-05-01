using SQLite;

namespace EventHub.Models
{
    public class FavoriteEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        [Indexed]
        public int EventId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool NeedsSync { get; set; } = true;
    }
}