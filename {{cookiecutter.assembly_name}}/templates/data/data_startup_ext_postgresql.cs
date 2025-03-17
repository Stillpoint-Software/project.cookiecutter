  public static IServiceCollection AddProvider( this IServiceCollection services, IConfiguration config, Serilog.ILogger logger = null )
    {
        var connectionString = config["PostgreSql:ConnectionString"]; // from appsettings.<ENV>.json

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