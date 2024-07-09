public class Event
{
    private static int _reporterCounter = 0;

    public int ReporterId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int MetricId { get; private set; }
    public int MetricValue { get; private set; }
    public string Message { get; set; }

    private static readonly Random _random = new Random();

    public Event()
    {
        ReporterId = ++_reporterCounter;
        Timestamp = DateTime.Now;
        MetricId = _random.Next(1, 11);
        MetricValue = _random.Next(1, 101);
        Message = "hello world";
    }
}
