{%- if cookiecutter.database == "Postgresql" -%}
using Npgsql;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

public interface IDbConnectionProvider
{
NpgsqlConnection GetConnection();
}

public class NpgsqlConnectionProvider : IDbConnectionProvider
{
private string ConnectionString { get; }

public NpgsqlConnectionProvider( string connectionString ) 
{
    ConnectionString = connectionString;
}

public NpgsqlConnection GetConnection() => new (ConnectionString);
}

{%- elif cookiecutter.database == "MongoDb" -%}
using MongoDb.Driver;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

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

{% endif %}
