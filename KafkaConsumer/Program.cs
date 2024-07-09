using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System.IO;
using MongoDB.Driver;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("config.yaml")
            .Build();

        string bootstrapServers = config["Kafka:BootstrapServers"];
        string topic = config["Kafka:Topic"];

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "event-group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        var mongoConnectionString = config["MongoDB:ConnectionString"];
        var mongoClient = new MongoClient(mongoConnectionString);
        var databaseName = config["MongoDB:Database"];
        var collectionName = config["MongoDB:Collection"];
        var database = mongoClient.GetDatabase(databaseName);
        var collection = database.GetCollection<EventData>(collectionName);

        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe(topic);
            Console.WriteLine("Starting consumption loop...");
            try
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Waiting to consume...");
                        var cr = consumer.Consume();

                        // Parse Kafka message into EventData object
                        EventData eventData = JsonConvert.DeserializeObject<EventData>(cr.Message.Value);

                        // Insert into MongoDB
                        collection.InsertOne(eventData);
                        Console.WriteLine($"Inserted event '{eventData}' into MongoDB.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.Error.WriteLine("Consumption loop cancelled: " + e.Message);
                        break;
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
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
