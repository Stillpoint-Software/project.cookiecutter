using MongoDB.Driver;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public interface IMigrationDatabaseProvider
{
    IMongoDatabase GetDatabase();
}

public class MigrationDatabaseProvider : IMigrationDatabaseProvider
{
    private readonly IMongoDatabase _database;

    public MigrationDatabaseProvider( IMongoDatabase database )
    {
        _database = database;
    }

    public MigrationDatabaseProvider( string connectionString, string databaseName )
    {
        var client = new MongoClient( connectionString );
        var database = client.GetDatabase( databaseName );

        _database = database;
    }

    public IMongoDatabase GetDatabase()
    {
        return _database;
    }
}
