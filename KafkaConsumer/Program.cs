class Program
{
    static void Main(string[] args)
    {
        var configHandler = new ConfigurationHandler();

        string bootstrapServers = configHandler.GetKafkaBootstrapServers();
        string topic = configHandler.GetKafkaTopic();
        string mongoConnectionString = configHandler.GetMongoConnectionString();
        string mongoDatabaseName = configHandler.GetMongoDatabaseName();
        string mongoCollectionName = configHandler.GetMongoCollectionName();

        var mongoDbHandler = new MongoDbHandler(mongoConnectionString, mongoDatabaseName, mongoCollectionName);
        var kafkaConsumer = new KafkaConsumer(bootstrapServers, topic, mongoDbHandler);

        kafkaConsumer.Start();
    }
}
