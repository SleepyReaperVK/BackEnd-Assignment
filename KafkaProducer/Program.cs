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
        var configLoader = new ConfigLoader();
        var kafkaProducer = new KafkaProducer(configLoader);

        await kafkaProducer.StartProducing();
    }
}