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
        int reporterId = _reporterCounter;  
        _reporterCounter=_reporterCounter+ configLoader.GetInitIncrement();

        DateTime timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        int metricId = _random.Next(configLoader.GetEventMatricIdStart(), configLoader.GetEventMatricIdEnd());
        int metricValue = _random.Next(configLoader.GetEventMatricValueStart(), configLoader.GetEventMatricValueEnd());
        string message = configLoader.GetEventMessage();
        

        return new Event(reporterId, timestamp, metricId, metricValue, message);
    }
}
