using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Extensions;

internal static class {{cookiecutter.database}}Extensions
{
    public static void AddMongoDb( this IServiceCollection services, IConfiguration config )
    {
        // MongoDB is already a pool connection, so if you don’t use a Singleton
        // a new pool connection will always be created.

        var connectionString = config["MongoDb:ConnectionString"];
        var databaseName = config["MongoDb:Database"];
        {% if cookiecutter.include_azure == "yes" %}
        var keyVaultNameSpace = config["MongoDb:KeyVaultNamespace"];
          services.AddSingleton<IMongoDbService, MongoDbService>(
            c => new MongoDbService( connectionString, databaseName, keyVaultNameSpace )
        );
        {% endif %}
          services.AddSingleton<IMongoDbService, MongoDbService>(
            c => new MongoDbService( connectionString, databaseName, null )
        );
      
    }
}
