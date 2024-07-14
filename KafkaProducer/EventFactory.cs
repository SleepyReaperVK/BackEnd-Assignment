public class EventFactory
{
    private static int _reporterCounter; 
    private static Random _random = new Random();
 public static TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
    public static Event CreateEvent(ConfigLoader configLoader)
    {
        if (_reporterCounter == 0)
        {
            _reporterCounter = configLoader.GetInitIndex();
        }
        int reporterId = _reporterCounter + 1; //TODO 
        DateTime timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        int metricId = _random.Next(1, 11);
        int metricValue = _random.Next(1, 101);
        string message = "hello world";

        return new Event(reporterId, timestamp, metricId, metricValue, message);
    }
}
