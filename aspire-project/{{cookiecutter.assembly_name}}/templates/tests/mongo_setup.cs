private readonly IMongoDatabase _database;

public CreateSampleCommandTests()
{
    // Initialize your MongoDB connection here (connect to your running container)
    var connectionString = "mongodb://test:test@mongodb:27017/";
    var client = new MongoClient(connectionString);
    _database = client.GetDatabase("test");
}
