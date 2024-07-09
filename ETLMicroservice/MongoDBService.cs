using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ETLMicroservice
{
    //Manages MongoDB operations.
    public class MongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _mongoCollection;

        public MongoDBService(string connectionString, string databaseName, string collectionName)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
            var client = new MongoClient(settings);
            var database = client.GetDatabase(databaseName);
            _mongoCollection = database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task CreateIndexAsync()
        {
            var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Descending("Timestamp");
            await _mongoCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(indexKeysDefinition));
        }

        public async Task<IList<BsonDocument>> FetchDataAsync(DateTime filterTimestamp)
        {
            var filter = Builders<BsonDocument>.Filter.Gte("Timestamp", filterTimestamp);
            var sort = Builders<BsonDocument>.Sort.Ascending("Timestamp");
            var options = new FindOptions<BsonDocument> { Sort = sort };

            var cursor = await _mongoCollection.FindAsync(filter, options);
            return await cursor.ToListAsync();
        }
    }
}
