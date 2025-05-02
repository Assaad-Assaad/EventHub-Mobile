using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class UserEvent
    {
        [PrimaryKey]
        public int Id { get; set; }

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
    }
}
