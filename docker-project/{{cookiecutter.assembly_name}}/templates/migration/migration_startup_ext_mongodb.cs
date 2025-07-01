public static IServiceCollection AddMongoDbProvider(this IServiceCollection services, IConfiguration config, Serilog.ILogger logger = null)
{
    var connectionString = config["{{cookiecutter.assembly_name}}:ConnectionString"]; // from appsettings.<ENV>.json

    // Note: do not log sensitive data
    //logger?.Information( $"Connecting to `{connectionString}`." );

    services.AddTransient<IMongoClient, MongoClient>(_ => new MongoClient(connectionString));

    return services;
}

public static IServiceCollection AddMongoDbMigrations(this IServiceCollection services, IConfiguration config)
{
    var lockingEnabled = config.GetValue<bool>("Migrations:Lock:Enabled");
    var lockName = config["Migrations:Lock:Name"];
    var lockMaxLifetime = TimeSpan.FromSeconds(config.GetValue("Migrations:Lock:MaxLifetime", 3600));

    var profiles = (IList<string>)config.GetSection("Migrations:Profiles")
        .Get<IEnumerable<string>>() ?? Enumerable.Empty<string>()
        .ToList();

    var databaseName = config.GetValue<string>("Migrations:DatabaseName");
    var collectionName = config.GetValue<string>("Migrations:CollectionName");

    services.AddMongoDBMigrations(c =>
    {
        c.Profiles = profiles;
        c.LockName = lockName;
        c.LockingEnabled = lockingEnabled;
        c.LockMaxLifetime = lockMaxLifetime;

        c.DatabaseName = databaseName;
        c.CollectionName = collectionName;
    });

    return services;
}

internal static LoggerConfiguration AddMongoDbFilters(this LoggerConfiguration self)
{
    var mongoDbLevelSwitch = new LoggingLevelSwitch();
    self.MinimumLevel.Override("{{cookiecutter.assembly_name}}", mongoDbLevelSwitch);

    mongoDbLevelSwitch.MinimumLevel = LogEventLevel.Warning;
    return self;
}