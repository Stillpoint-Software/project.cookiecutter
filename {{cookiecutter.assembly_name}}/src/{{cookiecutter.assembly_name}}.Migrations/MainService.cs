using System.Reflection;
using System.Text.RegularExpressions;
using DbUp;
using DbUp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Migrations;

public class MainService : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MainService( IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, ILogger<MainService> logger )
    {
        _applicationLifetime = applicationLifetime;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        await Task.Yield(); // yield to allow startup logs to write to console

        try
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var logFactory = provider.GetRequiredService<ILoggerFactory>();

            await RunMigrationsAsync( config, logFactory, stoppingToken );
        }
        catch ( Exception ex )
        {
            _logger.LogCritical( ex, "Migrations encountered an unhandled exception." );
        }

        _applicationLifetime.StopApplication();
    }

    private static async Task RunMigrationsAsync( IConfiguration configuration, ILoggerFactory logFactory, CancellationToken stoppingToken )
    {
        var connectionString = configuration["{{cookiecutter.database}}:ConnectionString"];
        var reset = configuration.GetValue<bool>( "Runner:HardReset" );
        var logger = logFactory.CreateLogger( "Migrations" );

        logger.LogInformation( "Migrating {{cookiecutter.database}} database." );

        logger.LogDebug( $"Connection: {connectionString}" );
        logger.LogInformation( $"HardReset: {reset}" );

        // this will ensure that the database is created
        EnsureDatabase.For.PostgresqlDatabase( connectionString );

        var assemblyName = Assembly.GetAssembly( typeof( MainService ) )!.GetName().Name;
        var scriptRegex = new Regex(
            @$"{assemblyName}\.Scripts\.Script\d{{4}}-.+\.sql",
            RegexOptions.IgnoreCase
        );

        if ( reset )
        {
            var resetRunner = DeployChanges.To
                .PostgresqlDatabase( connectionString )
                .WithScriptsEmbeddedInAssembly( Assembly.GetExecutingAssembly(), ResetFilter( assemblyName ) )
                .JournalTo( new NullJournal() )
                .LogToConsole()
                .Build();

            var resetResults = resetRunner.PerformUpgrade();

            if ( !resetResults.Successful )
                throw new Exception( "An error occurred while migrating the {{cookiecutter.database}} database", resetResults.Error );
        }

        var upgrader = DeployChanges.To
            .PostgresqlDatabase( connectionString )
            .WithScriptsEmbeddedInAssembly( Assembly.GetExecutingAssembly(), MigrationFilter( assemblyName, scriptRegex ) )
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if ( !result.Successful )
            throw new Exception( "An error occurred while migrating the {{cookiecutter.database}} database", result.Error );

        logger.LogInformation( "Migrated {{cookiecutter.database}} database." );

        await Task.CompletedTask;
    }

    private static Func<string, bool> ResetFilter( string assemblyName )
    {
        return migration => string.Equals( migration, $"{assemblyName}.Scripts.HardReset.sql", StringComparison.OrdinalIgnoreCase );
    }

    private static Func<string, bool> MigrationFilter( string assemblyName, Regex scriptPattern )
    {
        return migration =>
            !string.Equals( migration, $"{assemblyName}.Scripts.HardReset.sql", StringComparison.OrdinalIgnoreCase ) || scriptPattern.IsMatch( migration );
    }
}
