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
