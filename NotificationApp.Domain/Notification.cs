namespace NotificationApp.Domain
{
    public class Notification
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public NotificationLevel Level { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
   
}
public enum NotificationLevel
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}
