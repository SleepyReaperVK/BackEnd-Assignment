using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ETLMicroservice
{
    class Program
    {
        private static Timer _timer;
        private static IMongoCollection<BsonDocument> _mongoCollection;
        private static ConnectionMultiplexer _redisConnection;
        private static IDatabase _redisDatabase;
        private static IConfigurationRoot _configuration;

        static async Task Main(string[] args)
        {
            // Set up configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            // Get MongoDB settings from config
            string mongoConnectionString = _configuration["MongoDB:ConnectionString"];
            string mongoDatabaseName = _configuration["MongoDB:DataBase"];
            string mongoCollectionName = _configuration["MongoDB:Collection"];

            // Set up MongoDB client
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
            _mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoCollectionName);

            // Get Redis settings from config
            string redisConnectionString = _configuration["Redis:ConnectionString"];
            _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisDatabase = _redisConnection.GetDatabase();

            // Set up and start the timer
            _timer = new Timer(30000); // 30 seconds
            _timer.Elapsed += async (sender, e) => await OnTimedEvent();
            _timer.AutoReset = true;
            _timer.Enabled = true;

            Console.WriteLine("ETL microservice started. Press [Enter] to exit.");
            Console.ReadLine();
        }

        private static async Task OnTimedEvent()
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Empty;
                var sort = Builders<BsonDocument>.Sort.Descending("timestamp");
                var options = new FindOptions<BsonDocument>
                {
                    Limit = 30,
                    Sort = sort
                };

                var mongoEntries = await _mongoCollection.FindAsync(filter, options);
                var entriesList = await mongoEntries.ToListAsync();

                var redisEntries = new List<HashEntry>();
                foreach (var entry in entriesList)
                {
                    redisEntries.Add(new HashEntry(entry["_id"].ToString(), entry.ToJson()));
                }

                await _redisDatabase.HashSetAsync("last30Entries", redisEntries.ToArray());

                Console.WriteLine("Transferred 30 entries from MongoDB to Redis.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during ETL process: {ex.Message}");
            }
        }
    }
}
