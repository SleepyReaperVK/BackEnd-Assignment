using MongoDB.Bson;
using MongoDB.Driver;
using System;

public class MongoDbHandler
{
    private readonly IMongoCollection<EventData> _collection;

    public MongoDbHandler(string connectionString, string databaseName, string collectionName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<EventData>(collectionName);
    }

    public void InsertEvent(EventData eventData)
    {
        var filter = Builders<EventData>.Filter.Eq(e => e.ReporterId, eventData.ReporterId);
        var existingEvent = _collection.Find(filter).FirstOrDefault();

        if (existingEvent != null)
        {
            throw new DuplicateEventException($"Event with ID '{eventData.ReporterId}' already exists.");
        }

        _collection.InsertOne(eventData);
    }
}

public class DuplicateEventException : Exception
{
    public DuplicateEventException(string message) : base(message) { }
}
