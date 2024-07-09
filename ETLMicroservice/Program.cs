using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using System;
using System.IO;
using System.Threading.Tasks;
using Timer=System.Timers.Timer;

namespace ETLMicroservice
{
    class Program
    {
        private static IMongoCollection<BsonDocument> _mongoCollection;
        private static IDatabase _redisDatabase;
        private static string _lastTimestampKey = "lastTimestamp";

        static async Task Main(string[] args)
        {
            // Set up configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            // Get MongoDB settings from config
            string mongoConnectionString = configuration["MongoDB:ConnectionString"];
            string mongoDatabaseName = configuration["MongoDB:DataBase"];
            string mongoCollectionName = configuration["MongoDB:Collection"];

            // Set up MongoDB client with options
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30); // Adjust connection idle time
            var mongoClient = new MongoClient(settings);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
            _mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoCollectionName);

            // Ensure index usage
            var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Descending("Timestamp");
            await _mongoCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(indexKeysDefinition));

            // Get Redis settings from config
            string redisConnectionString = configuration["Redis:ConnectionString"];
            ConnectionMultiplexer redisConnection = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
            _redisDatabase = redisConnection.GetDatabase();

            // Check and output Redis connection status
            Console.WriteLine("Connected to Redis? " + redisConnection.IsConnected);
            //init run
            await OnTimedEvent();
            // Set up and start the timer
            Timer timer = new Timer(30000); // 30 seconds
            timer.Elapsed += async (sender, e) => await OnTimedEvent();
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.WriteLine("ETL microservice started. Press [Ctrl+C] to exit.");

            // Keep the application running until Ctrl+C is pressed
            await Task.Delay(-1);
        }

        private static async Task OnTimedEvent()
        {
            try
            {
                // Get the last timestamp from Redis
                var lastTimestamp = await _redisDatabase.StringGetAsync(_lastTimestampKey);
                DateTime filterTimestamp;

                if (lastTimestamp.IsNullOrEmpty)
                {
                    // Use someSecsAgo as fallback if no last timestamp found
                    DateTime currentTime = DateTime.Now;
                    DateTime someSecsAgo = currentTime.Subtract(TimeSpan.FromSeconds(30));
                    filterTimestamp = someSecsAgo;
                    Console.WriteLine("Using fallback timestamp: " + someSecsAgo);
                }
                else
                {
                    filterTimestamp = DateTime.Parse(lastTimestamp);
                    Console.WriteLine("Using last timestamp from Redis: " + filterTimestamp);
                }

                // Fetch data from MongoDB based on the filterTimestamp
                var filter = Builders<BsonDocument>.Filter.Gte("Timestamp", filterTimestamp);
                var sort = Builders<BsonDocument>.Sort.Ascending("Timestamp");
                var options = new FindOptions<BsonDocument>
                {
                    Sort = sort
                };

                var mongoCursor = await _mongoCollection.FindAsync(filter, options);
                var mongoEntries = await mongoCursor.ToListAsync();

                Console.WriteLine($"Retrieved {mongoEntries.Count} entries from MongoDB.");

                // Process each event and store in Redis
                foreach (var entry in mongoEntries)
                {
                    var reporterId = entry["ReporterId"].AsInt32.ToString(); // Example: Cast to string if needed
                    var timestamp = entry["Timestamp"].ToUniversalTime().ToString("yyyyMMddHHmmss");
                    var key = $"{reporterId}:{timestamp}";
                    var value = entry.ToJson();

                    bool isSet = await _redisDatabase.StringSetAsync(key, value);
                    Console.WriteLine($"isSet for key {key}: {isSet}");
                    if (isSet)
                    {
                        Console.WriteLine($"Successfully set key: {key} with value: {value}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to set key: {key}");
                    }
                }

                // Update last timestamp in Redis if entries were retrieved
                if (mongoEntries.Any())
                {
                    var latestTimestamp = mongoEntries.Max(e => e["Timestamp"].ToUniversalTime()).ToString("o"); // ISO 8601 format
                    await _redisDatabase.StringSetAsync(_lastTimestampKey, latestTimestamp);
                    Console.WriteLine("Updated last timestamp in Redis: " + latestTimestamp);
                }

                Console.WriteLine("Transferred entries from MongoDB to Redis.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during ETL process: {ex.Message}");
            }
        }
    }
}
