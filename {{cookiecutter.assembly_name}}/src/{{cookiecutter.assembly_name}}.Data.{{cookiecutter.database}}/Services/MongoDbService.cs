using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDatabase _database;

     public MongoDbService( string connectionString, string databaseName, string? keyVaultNameSpace )
    {
        var clientSettings = MongoClientSettings.FromConnectionString( connectionString );
        clientSettings.LinqProvider = LinqProvider.V3;

        var client = new MongoClient( clientSettings );
        _database = client.GetDatabase( databaseName );
    }

    public IMongoCollection<T> GetCollection<T>( string name )
    {
        var collectionName = (typeof( T ).GetCustomAttributes( typeof( BsonCollectionAttribute ), true )
                .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName;

        return _database.GetCollection<T>( collectionName ?? name );
    }
}
