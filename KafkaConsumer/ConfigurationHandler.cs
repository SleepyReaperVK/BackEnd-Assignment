using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System.IO;

public class ConfigurationHandler
{
    public IConfiguration Configuration { get; private set; }

    public ConfigurationHandler()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("config.yaml")
            .Build();
    }

    public string GetKafkaBootstrapServers() => Configuration["Kafka:BootstrapServers"];
    public string GetKafkaTopic() => Configuration["Kafka:Topic"];
    public string GetMongoConnectionString() => Configuration["MongoDB:ConnectionString"];
    public string GetMongoDatabaseName() => Configuration["MongoDB:Database"];
    public string GetMongoCollectionName() => Configuration["MongoDB:Collection"];
}
