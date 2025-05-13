

namespace EventHub.Models
{
    public class LoggedInUser
    {
        [PrimaryKey]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public DateTime TokenExpiration { get; set; }
    }
}
