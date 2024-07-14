using System;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace ETLMicroservice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationHandler = new ConfigurationHandler();

            var mongoService = new MongoDBService(
                configurationHandler.GetMongoConnectionString(),
                configurationHandler.GetMongoDatabaseName(),
                configurationHandler.GetMongoCollectionName(),
                configurationHandler
            );
            await mongoService.CreateIndexAsync();// set up mongo to use Timestamp as index

            var redisService = new RedisService(configurationHandler.GetRedisConnectionString()); // set up redis

            var etlService = new ETLService(mongoService, redisService, configurationHandler.GetRedisTimeKey());

            await etlService.ExecuteAsync();

            Timer timer = new Timer(configurationHandler.GetInitIncrementSec()); 
            timer.Elapsed += async (sender, e) => await etlService.ExecuteAsync();
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.WriteLine("ETL microservice started. Press [Ctrl+C] to exit.");
            await Task.Delay(configurationHandler.GetInitTaskDelay());
        }
    }
}
