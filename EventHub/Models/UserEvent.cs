using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class UserEvent
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }

        public bool IsFavorite { get; set; }
        public bool IsSignedIn { get; set; }

        [Indexed]
        public bool IsSynced { get; set; }

    }
}
