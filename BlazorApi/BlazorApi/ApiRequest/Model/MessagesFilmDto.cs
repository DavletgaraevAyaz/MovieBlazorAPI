namespace BlazorApi.ApiRequest.Model
{
    public class MessagesFilmDto
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string? Title { get; set; } // Название фильма
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
