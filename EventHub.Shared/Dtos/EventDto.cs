using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public class EventDto
    {

        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; }

        [StringLength(500)] 
        public string? Image { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(100)] 
        public string? Category { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

       
    }
}
