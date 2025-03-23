using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAPI.ApiRequest.Model
{
    public class Logins
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        public Users Users { get; set; }
    }
}
