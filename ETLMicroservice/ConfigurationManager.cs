using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System;
using System.IO;

public class ConfigurationHandler
{
    public IConfiguration Configuration { get; private set; }

    public ConfigurationHandler()
    {
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        try
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml", optional: false, reloadOnChange: true)
                .Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load configuration: {ex.Message}");
            throw;
        }
    }

    public string GetRedisConnectionString() => Configuration["Redis:ConnectionString"];
    public string GetRedisTimeKey() => Configuration["Redis:TimeKey"];
    public string GetMongoConnectionString() => Configuration["MongoDB:ConnectionString"];
    public string GetMongoDatabaseName() => Configuration["MongoDB:Database"];
    public string GetMongoCollectionName() => Configuration["MongoDB:Collection"];
    public int GetInitIncrementSec() => int.Parse(Configuration["InitValues:InitIncrementsSec"]);
    public int GetInitTaskDelay() => int.Parse(Configuration["InitValues:InitTaskDelay"]);
    public int GetInitTimeoutSec() => int.Parse(Configuration["MongoDB:InitTimeoutSec"]);

}
