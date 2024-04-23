using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;
using {{cookiecutter.assembly_name}}.Migrations.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Extensions;

internal static class {{cookiecutter.database}}Extensions
{
    public static void AddMongoDb( this IServiceCollection services, IConfiguration config )
    {
        // MongoDB is already a pool connection, so if you don’t use a Singleton
        // a new pool connection will always be created.

        var connectionString = config["Mongo:ConnectionString"];
        var databaseName = config["Mongo:Database"];
        {%- if cookiecutter.include_azure == "yes" -%}
        var keyVaultNameSpace = config["Mongo:KeyVaultNamespace"];
          services.AddSingleton<IMongoDbService, MongoDbService>(
            c => new MongoDbService( connectionString, databaseName, keyVaultNameSpace )
        );
        {% endif %}
          services.AddSingleton<IMongoDbService, MongoDbService>(
            c => new MongoDbService( connectionString, databaseName, null )
        );
      
    }

    public static void AddMongoDbMigrations( this IServiceCollection services, IConfiguration config )
    {
        var connectionString = config["Mongo:ConnectionString"];
        var databaseName = config["Mongo:Database"];

        services.AddSingleton<IMigrationDatabaseProvider>(
            c => new MigrationDatabaseProvider( connectionString, databaseName )
        );

        services.AddSingleton<IMigrationActivator, MigrationActivator>();
    }

}
