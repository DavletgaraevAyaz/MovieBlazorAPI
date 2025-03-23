namespace BlazorApi.ApiRequest.Model
{
    public class MessagesDto
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
