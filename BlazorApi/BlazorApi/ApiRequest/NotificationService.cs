using System;
using System.Collections.Generic;

public class NotificationService
{
    public event Action<NotificationModel> OnNotificationAdded;

    public void ShowSuccess(string message)
    {
        var notification = new NotificationModel
        {
            Message = message,
            Type = "success"
        };
        OnNotificationAdded?.Invoke(notification);
    }

    public void ShowError(string message)
    {
        var notification = new NotificationModel
        {
            Message = message,
            Type = "error"
        };
        OnNotificationAdded?.Invoke(notification);
    }

    public class NotificationModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public string Type { get; set; }
    }
}