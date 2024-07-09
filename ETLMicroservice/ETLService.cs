
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETLMicroservice
{
    //Orchestrates the ETL process.
    public class ETLService
    {
        private readonly MongoDBService _mongoDBService;
        private readonly RedisService _redisService;
        private readonly string _lastTimestampKey ;

        public ETLService(MongoDBService mongoDBService, RedisService redisService , string lastTimestampKey)
        {
            _mongoDBService = mongoDBService;
            _redisService = redisService;
            _lastTimestampKey = lastTimestampKey;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var lastTimestamp = await _redisService.GetLastTimestampAsync(_lastTimestampKey);
                Console.WriteLine("Using Redis timestamp: " + lastTimestamp);
                DateTime filterTimestamp = string.IsNullOrEmpty(lastTimestamp)
                    ? DateTime.Now.Subtract(TimeSpan.FromSeconds(30))
                    : DateTime.Parse(lastTimestamp);

                Console.WriteLine("Using timestamp: " + filterTimestamp);

                var mongoEntries = await _mongoDBService.FetchDataAsync(filterTimestamp);
                Console.WriteLine($"Retrieved {mongoEntries.Count} entries from MongoDB.");

                foreach (var entry in mongoEntries)
                {
                    var reporterId = entry["ReporterId"].AsInt32.ToString();
                    var timestamp = entry["Timestamp"].ToUniversalTime().ToString("yyyyMMddHHmmss");
                    var key = $"{reporterId}:{timestamp}";
                    var value = entry.ToJson();

                    var isSet = await _redisService.SetDataAsync(key, value);
                    Console.WriteLine($"isSet for key {key}: {isSet}");
                }

                if (mongoEntries.Any())
                {
                    var latestTimestamp = mongoEntries.Max(e => e["Timestamp"].ToUniversalTime()).ToString("o");
                    await _redisService.SetLastTimestampAsync(_lastTimestampKey, latestTimestamp);
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
