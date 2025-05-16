using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public class UserEventDto
    {
        public int UserId { get; set; }

        public int EventId { get; set; }



        public bool IsFavorite { get; set; }

        public bool IsSignedIn { get; set; }

        public string Role { get; set; } = "User";
    }
}
