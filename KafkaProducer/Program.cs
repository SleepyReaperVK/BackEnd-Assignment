using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // Load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml")
                .Build();

            string bootstrapServers = config["Kafka:BootstrapServers"];
            string topic = config["Kafka:Topic"];

            var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers };

            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                Console.WriteLine("Producer Started, Connecting...");
                while (true)
                {
                    var eventObj = new Event();
                    var eventJson = JsonSerializer.Serialize(eventObj);

                    try
                    {
                        var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = eventJson });
                        Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}: {eventJson}");
                    }
                    catch (ProduceException<Null, string> ex)
                    {
                        Console.WriteLine($"Failed to produce message: {ex.Error.Reason}");
                    }

                    Thread.Sleep(1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}