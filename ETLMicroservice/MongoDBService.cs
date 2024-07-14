using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ETLMicroservice
{
    //Manages MongoDB operations.
    public class MongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _mongoCollection;
        
        public MongoDBService(string connectionString, string databaseName, string collectionName , ConfigurationHandler configuration)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(configuration.GetInitTimeoutSec());
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
            var filter = Builders<BsonDocument>.Filter.Gt("Timestamp", filterTimestamp);
            var sort = Builders<BsonDocument>.Sort.Ascending("Timestamp");
            var options = new FindOptions<BsonDocument> { Sort = sort };
            //var emptyFilter = Builders<BsonDocument>.Filter.Empty;
            //var cursor = await _mongoCollection.FindAsync(emptyFilter,options);
            var cursor = await _mongoCollection.FindAsync(filter , options);//TODO

            return await cursor.ToListAsync();
        }
    }
}
