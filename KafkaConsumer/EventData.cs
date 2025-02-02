using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class EventData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int ReporterId { get; set; }
    public DateTime Timestamp { get; set; }
    public int MetricId { get; set; }
    public int MetricValue { get; set; }
    public string Message { get; set; }

    public EventData()
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
    }
}