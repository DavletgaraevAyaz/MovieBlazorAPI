namespace BlazorApi.ApiRequest.Model
{
    public class FilmChatDto
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
    }
}
