using System.ComponentModel.DataAnnotations;

namespace EventHub.Api.Entities
{
    public class Event
    {
        [Key]
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
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(100)] 
        public string? Category { get; set; }

        [StringLength(200)] 
        public string? Location { get; set; }

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();


    }
}
