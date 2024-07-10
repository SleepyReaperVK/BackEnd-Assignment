using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

public class KafkaProducer
{
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly ConfigLoader _configLoader;

    public KafkaProducer(ConfigLoader configLoader)
    {
        _bootstrapServers = configLoader.GetBootstrapServers();
        _topic = configLoader.GetTopic();
        _configLoader = configLoader;
    }

    public async Task StartProducing()
    {
        var producerConfig = new ProducerConfig { BootstrapServers = _bootstrapServers };

        using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
        {
            Console.WriteLine("Producer Started, Connecting...");
            //await ProduceMessage(producer, "Initial connection test message");
            // wait for connection
            Thread.Sleep(500);
            while (true)
            {
                var eventObj = new Event(_configLoader);
                var eventJson = JsonSerializer.Serialize(eventObj);

                await ProduceMessage(producer, eventJson);

                Thread.Sleep(1000);
            }
        }
    }

    private async Task ProduceMessage(IProducer<Null, string> producer, string message)
    {
        try
        {
            var deliveryResult = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
            Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}: {message}");
        }
        catch (ProduceException<Null, string> ex)
        {
            Console.WriteLine($"Failed to produce message: {ex.Error.Reason}");
        }
    }
}
