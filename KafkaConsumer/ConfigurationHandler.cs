using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System.IO;

public class ConfigurationHandler
{
    public IConfiguration Configuration { get; private set; }

    public  ConfigurationHandler()
    {
        LoadConfiguration();
    }
    private void LoadConfiguration()
    {
        try
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml")
                .Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load configuration: {ex.Message}");
            throw;
        }
    }

    public string GetKafkaBootstrapServers() => Configuration["Kafka:BootstrapServers"];
    public string GetKafkaTopic() => Configuration["Kafka:Topic"];
    public string GetMongoConnectionString() => Configuration["MongoDB:ConnectionString"];
    public string GetMongoDatabaseName() => Configuration["MongoDB:Database"];
    public string GetMongoCollectionName() => Configuration["MongoDB:Collection"];
}
