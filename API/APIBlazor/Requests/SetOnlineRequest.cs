using System.ComponentModel.DataAnnotations;

namespace APIBlazor.Requests
{
    public class SetOnlineRequest
    {
        [Required]
        public int userId { get; set; }

        [Required]
        public string connectionId { get; set; }
    }
}
