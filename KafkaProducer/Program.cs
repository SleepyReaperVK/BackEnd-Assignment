using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System.IO;

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
                int messageId = 0;
                Console.WriteLine("Produser Started, Connecting...");
                while (true)
                {
                    var timestamp = DateTime.UtcNow;
                    var message = $"{messageId},{timestamp:O}";

                    try
                    {
                        var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                        Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}: {message}");
                    }
                    catch (ProduceException<Null, string> ex)
                    {
                        Console.WriteLine($"Failed to produce message: {ex.Error.Reason}");
                    }

                    messageId++;
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
