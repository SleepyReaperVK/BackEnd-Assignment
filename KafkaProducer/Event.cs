public class Event
{
    private static int _reporterCounter; 
    private static Random _random = new Random();

    public int ReporterId { get; set; }
    public DateTime Timestamp { get; set; }
    public int MetricId { get; set; }
    public int MetricValue { get; set; }
    public string Message { get; set; }
    public static TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
    public Event(ConfigLoader configLoader)
    {
        if (_reporterCounter == 0)
        {
            _reporterCounter = configLoader.GetInitIndex();
        }
        ReporterId = _reporterCounter++;
        Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        MetricId = _random.Next(1, 11);
        MetricValue = _random.Next(1, 101);
        Message = "hello world";
    }
}