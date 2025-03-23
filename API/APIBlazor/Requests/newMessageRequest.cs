using System.ComponentModel.DataAnnotations;

namespace APIBlazor.Requests
{
    public class newMessageRequest
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
    }
}
