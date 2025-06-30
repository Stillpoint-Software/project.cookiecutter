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
        var npgsqlLevelSwitch = new LoggingLevelSwitch();
        self.MinimumLevel.Override( "MongoDB", npgsqlLevelSwitch );

        npgsqlLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        return self;
    }