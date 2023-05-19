namespace MyLiteMafia.Common.Events
{
    public class GeofenceNotificationEventArgs : EventArgs
    {
        public string Notification { get; set; }

        public GeofenceNotificationEventArgs(string notification)
        {
            Notification = notification;
        }
    }
}
