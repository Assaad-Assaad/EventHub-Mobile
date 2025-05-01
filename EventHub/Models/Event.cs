using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        
        public DateTime Date { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }


        [Ignore]
        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();


        




    }





   
}
