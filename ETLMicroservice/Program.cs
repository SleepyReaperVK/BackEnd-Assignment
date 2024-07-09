using System;
using System.Threading.Tasks;
using Timer=System.Timers.Timer;

namespace ETLMicroservice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = ConfigurationManager.LoadConfiguration();

            var mongoService = new MongoDBService(
                configuration["MongoDB:ConnectionString"],
                configuration["MongoDB:DataBase"],
                configuration["MongoDB:Collection"]
            );
            await mongoService.CreateIndexAsync();

            var redisService = new RedisService(configuration["Redis:ConnectionString"]);

            var etlService = new ETLService(mongoService, redisService, configuration["Redis:TimeKey"]);

            await etlService.ExecuteAsync();

            Timer timer = new Timer(30000); // 30 seconds
            timer.Elapsed += async (sender, e) => await etlService.ExecuteAsync();
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.WriteLine("ETL microservice started. Press [Ctrl+C] to exit.");
            await Task.Delay(-1);
        }
    }
}
