using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;

public class ConfigLoader
{
    public IConfiguration Configuration { get; private set; }

    public ConfigLoader()
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

    public string GetBootstrapServers()
    {
        return Configuration["Kafka:BootstrapServers"];
    }

    public string GetTopic()
    {
        return Configuration["Kafka:Topic"];
    }

    public int GetInitIndex()
    {
        return int.Parse(Configuration["InitValues:InitRepIndex"]);
    }

    public int GetInitIncrement()
    {
        return int.Parse(Configuration["InitValues:InitRepIncrement"]);
    }

    public int GetInitSleepSeconds()
    {
        return int.Parse(Configuration["InitValues:InitSleepSec"]);
    }

    public int GetInitIncrementSeconds()
    {
        return int.Parse(Configuration["InitValues:InitIncrementsSec"]);
    }

    public int GetEventMatricIdStart()
    {
        return int.Parse(Configuration["EventValues:EventMatricIdRndStart"]);
    }

    public int GetEventMatricIdEnd()
    {
        return int.Parse(Configuration["EventValues:EventMatricIdRndStartEnd"]);
    }

    public int GetEventMatricValueStart()
    {
        return int.Parse(Configuration["EventValues:EventMatricValueRndStart"]);
    }

    public int GetEventMatricValueEnd()
    {
        return int.Parse(Configuration["EventValues:EventMatricValueRndStartEnd"]);
    }

    public string GetEventMessage()
    {
        return Configuration["EventValues:EventMassage"];
    }
}
