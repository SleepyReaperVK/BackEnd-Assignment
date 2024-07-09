using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ETLMicroservice
{
    //Manages Redis operations.
    public class RedisService
    {
        private readonly IDatabase _database;

        public RedisService(string connectionString)
        {
            var redisConnection = ConnectionMultiplexer.Connect(connectionString);
            _database = redisConnection.GetDatabase();
            Console.WriteLine("Connected to Redis? " + redisConnection.IsConnected);
        }

        public async Task<string> GetLastTimestampAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task<bool> SetLastTimestampAsync(string key, string value)
        {
            return await _database.StringSetAsync(key, value);
        }

        public async Task<bool> SetDataAsync(string key, string value)
        {
            return await _database.StringSetAsync(key, value);
        }
    }
}
