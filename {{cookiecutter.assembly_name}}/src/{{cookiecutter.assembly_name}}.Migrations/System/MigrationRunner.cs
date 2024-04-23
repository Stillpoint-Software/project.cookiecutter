using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

internal enum Direction
{
    Up,
    Down
}

public sealed class MigrationRunner
{
    private readonly IMigrationRepository _migrations;
    private readonly IMigrationLocator _locator;
    private readonly ILogger _logger;
    private string? database;
    private Assembly assembly;

    public MigrationRunner( IMongoDatabase database, IMigrationActivator activator, string migrationAssemblyName )
        : this( database, activator, null, migrationAssemblyName )
    {
    }

    public MigrationRunner( IMongoDatabase database, IMigrationActivator activator, Assembly migrationAssembly )
        : this( database, activator, null, migrationAssembly )
    {
    }

    public MigrationRunner( IMongoDatabase database, IMigrationActivator activator, ILogger logger, string migrationAssemblyName )
    {
        _migrations = new MigrationRepository( database );
        _locator = new MigrationLocator( migrationAssemblyName, activator );
        _logger = logger;
    }

    public MigrationRunner( IMongoDatabase database, IMigrationActivator activator, ILogger logger, Assembly migrationAssembly )
    {
        _migrations = new MigrationRepository( database );
        _locator = new MigrationLocator( migrationAssembly, activator );
        _logger = logger;
    }

    public MigrationRunner( IServiceProvider services, Assembly migrationAssembly )
    {
        var logger = services.GetService<ILogger<MigrationRunner>>();
        var activator = services.GetService<IMigrationActivator>();
        var provider = services.GetService<IMigrationDatabaseProvider>();
        var database = provider?.GetDatabase();

        _migrations = new MigrationRepository( database );
        _locator = new MigrationLocator( migrationAssembly, activator );
        _logger = logger;
    }

    public MigrationRunner( string? database, Assembly assembly )
    {
        this.database = database;
        this.assembly = assembly;
    }

    public async Task UpAsync( long version = long.MaxValue )
    {
        await RunAsync( version, Direction.Up );
    }

    public async Task DownAsync( long version = -1 )
    {
        await RunAsync( version, Direction.Down );
    }

    private async Task RunAsync( long version, Direction direction )
    {
        var assemblyName = _locator.Assembly.FullName;

        _logger?.LogInformation( "Discovering migrations in {Assembly}.", assemblyName );

        var migrations = await DiscoverMigrationsAsync( version, direction );

        _logger?.LogInformation( "Found {Count} migrations in {Assembly}.", migrations.Count, assemblyName );

        try
        {
            foreach ( var migration in migrations )
            {
                var record = new MigrationRecord( migration );
                _logger?.LogInformation( "Applying {Migration}.", record );

                switch ( direction )
                {
                    case Direction.Up:
                        await migration.UpAsync();
                        await _migrations.ApplyMigrationAsync( record );
                        break;

                    case Direction.Down:
                        await migration.DownAsync();
                        await _migrations.RemoveMigrationAsync( record );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException( nameof( direction ), direction, null );
                }

                _logger?.LogInformation( "Applied {Migration}.", record );
            }
        }
        catch ( Exception ex )
        {
            throw new MigrationException( $"Fatal error running {nameof( DownAsync )} migrations.", ex );
        }
    }

    private async Task<IList<Migration>> DiscoverMigrationsAsync( long version, Direction direction )
    {
        var records = await _migrations.GetMigrationRecordsAsync();

        return _locator.GetMigrations( direction, version )
            .Where( x =>
            {
                // filter migrations based on history and direction
                // to get the set of migrations we want to execute
                return direction == Direction.Up
                    ? records.All( r => r.Version != x.GetAttribute().Version )
                    : records.Any( r => r.Version == x.GetAttribute().Version );
            } )
            .ToList();
    }
}
