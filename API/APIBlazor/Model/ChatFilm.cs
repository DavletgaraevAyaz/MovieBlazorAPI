using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIBlazor.Model
{
    public class ChatFilm
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int SenderId { get; set; }

        [ForeignKey("Movie")]
        public int? MovieId { get; set; } = null;

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

        public Users Users { get; set; }

        public Movie Movie { get; set; }
    }
}
