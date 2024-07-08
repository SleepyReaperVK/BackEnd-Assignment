using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System.IO;
using MongoDB.Driver;

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
            GroupId = "test-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        var mongoConnectionString = config["MongoDB:ConnectionString"];
        var mongoClient = new MongoClient(mongoConnectionString);
        var databaseName = config["MongoDB:Database"];
        var collectionName = config["MongoDB:Collection"];
        var database = mongoClient.GetDatabase(databaseName);
        var collection = database.GetCollection<MyDataObject>(collectionName);

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

                        // Parse Kafka message into MyDataObject
                        var messageParts = cr.Message.Value.Split(',');
                        if (messageParts.Length == 2)
                        {
                            var id = int.Parse(messageParts[0]);
                            var timestamp = DateTime.Parse(messageParts[1]);

                            var dataObject = new MyDataObject
                            {
                                Id = id,
                                Timestamp = timestamp,
                                // You can add more fields here as needed
                            };

                            // Insert into MongoDB
                            collection.InsertOne(dataObject);
                            Console.WriteLine($"Inserted data '{dataObject}' into MongoDB.");
                        }
                        else
                        {
                            Console.WriteLine($"Invalid message format: '{cr.Message.Value}'");
                        }
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

    public class MyDataObject
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        // Add more properties as needed
    }
}
