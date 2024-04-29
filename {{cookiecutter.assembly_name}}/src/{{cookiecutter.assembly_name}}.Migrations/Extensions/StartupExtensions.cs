using Microsoft.Extensions.Configuration;
{% if cookiecutter.database == "Postgresql" %}
using Hyperbee.Migrations.Providers.Postgres;
{% elif cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Migrations.Extensions;

internal static class StartupExtensions
{
    internal static IConfigurationBuilder AddAppSettingsFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );
    }

    internal static IConfigurationBuilder AddAppSettingsEnvironmentFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( ConfigurationHelper.EnvironmentAppSettingsName, optional: true );
    }

{% if cookiecutter.database == "Postgresql" %}
  public static IServiceCollection AddProvider( this IServiceCollection services, IConfiguration config, ILogger logger = null )
    {
        var connectionString = config["Postgresql:ConnectionString"]; // from appsettings.<ENV>.json

        //Note: do not log sensitive data
        //logger?.Information( $"Connecting to `{connectionString}`." );

        object value = services.AddNpgsqlDataSource( connectionString );

        return services;
    }

    public static IServiceCollection AddMigrations( this IServiceCollection services, IConfiguration config )
    {
        var lockingEnabled = config.GetValue<bool>( "Migrations:Lock:Enabled" );
        var lockName = config["Migrations:Lock:Name"];
        var lockMaxLifetime = TimeSpan.FromSeconds( config.GetValue( "Migrations:Lock:MaxLifetime", 3600 ) );

        var profiles = (IList<string>) config.GetSection( "Migrations:Profiles" )
            .Get<IEnumerable<string>>() ?? Enumerable.Empty<string>()
            .ToList();

        var schemaName = config.GetValue<string>( "Migrations:SchemaName" );
        var tableName = config.GetValue<string>( "Migrations:TableName" );

        services.AddPostgresMigrations( c =>
        {
            c.Profiles = profiles;
            c.LockName = lockName;
            c.LockingEnabled = lockingEnabled;
            c.LockMaxLifetime = lockMaxLifetime;

            c.SchemaName = schemaName;
            c.TableName = tableName;
        } );

        return services;
    }

    internal static LoggerConfiguration AddFilters( this LoggerConfiguration self )
    {
        var npgsqlLevelSwitch = new LoggingLevelSwitch();
        self.MinimumLevel.Override( "Npgsql", npgsqlLevelSwitch );

        npgsqlLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        return self;
    }
{% elif cookiecutter.database == "MongoDb" %}
     public static IServiceCollection AddMongoDbProvider( this IServiceCollection services, IConfiguration config, ILogger logger = null )
    {
        var connectionString = config["{{cookiecutter.assembly_name}}:ConnectionString"]; // from appsettings.<ENV>.json

        // Note: do not log sensitive data
        //logger?.Information( $"Connecting to `{connectionString}`." );

        services.AddTransient<IMongoClient, MongoClient>( _ => new MongoClient( connectionString ) );

        return services;
    }

    public static IServiceCollection AddMongoDbMigrations( this IServiceCollection services, IConfiguration config )
    {
        var lockingEnabled = config.GetValue<bool>( "Migrations:Lock:Enabled" );
        var lockName = config["Migrations:Lock:Name"];
        var lockMaxLifetime = TimeSpan.FromSeconds( config.GetValue( "Migrations:Lock:MaxLifetime", 3600 ) );

        var profiles = (IList<string>) config.GetSection( "Migrations:Profiles" )
            .Get<IEnumerable<string>>() ?? Enumerable.Empty<string>()
            .ToList();

        var databaseName = config.GetValue<string>( "Migrations:DatabaseName" );
        var collectionName = config.GetValue<string>( "Migrations:CollectionName" );

        services.AddMongoDBMigrations( c =>
        {
            c.Profiles = profiles;
            c.LockName = lockName;
            c.LockingEnabled = lockingEnabled;
            c.LockMaxLifetime = lockMaxLifetime;

            c.DatabaseName = databaseName;
            c.CollectionName = collectionName;
        } );

        return services;
    }

    internal static LoggerConfiguration AddMongoDbFilters( this LoggerConfiguration self )
    {
        var mongoDbLevelSwitch = new LoggingLevelSwitch();
        self.MinimumLevel.Override( "{{cookiecutter.assembly_name}}", mongoDbLevelSwitch );

        mongoDbLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        return self;
    }
    {% endif %}
}

internal static class ConfigurationHelper
{
    internal static string EnvironmentAppSettingsName => $"appsettings.{Environment.GetEnvironmentVariable( "DOTNET_ENVIRONMENT" ) ?? "Development"}.json";
}
