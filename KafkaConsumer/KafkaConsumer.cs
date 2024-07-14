using Confluent.Kafka;
using Newtonsoft.Json;
using System;

public class KafkaConsumer
{
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly MongoDbHandler _mongoDbHandler;

    public KafkaConsumer(string bootstrapServers, string topic, MongoDbHandler mongoDbHandler)
    {
        _bootstrapServers = bootstrapServers;
        _topic = topic;
        _mongoDbHandler = mongoDbHandler;
    }

    public void Start()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "event-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe(_topic);
            Console.WriteLine("Starting consumption loop...");
            try
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Waiting to consume...");
                        var cr = consumer.Consume();
                        EventData eventData = JsonConvert.DeserializeObject<EventData>(cr.Message.Value);
                        _mongoDbHandler.InsertEvent(eventData);
                        Console.WriteLine($"Inserted event '{eventData.ReporterId}:{eventData.Timestamp}' into MongoDB.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine("Abort Inserting Event...");
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.WriteLine("Abort Inserting Event...");
                        Console.Error.WriteLine("Consumption loop cancelled: " + e.Message);
                        break;
                    }
                    catch (DuplicateEventException ex)
                    {
                        Console.WriteLine("Abort Inserting Event...");
                        Console.WriteLine($"Duplicate event id error: {ex.Message}");
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("Abort Inserting Event...");
                        Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Abort Inserting Event...");
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
