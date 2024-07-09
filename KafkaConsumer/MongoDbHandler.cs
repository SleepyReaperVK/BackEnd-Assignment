using MongoDB.Driver;

public class MongoDbHandler
{
    private readonly IMongoCollection<EventData> _collection;

    public MongoDbHandler(string connectionString, string databaseName, string collectionName)
    {
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(databaseName);
        _collection = database.GetCollection<EventData>(collectionName);
    }

    public void InsertEvent(EventData eventData)
    {
        _collection.InsertOne(eventData);
    }
}
