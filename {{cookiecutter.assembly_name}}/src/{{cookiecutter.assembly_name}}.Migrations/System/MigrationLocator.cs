using System.Reflection;
using {{cookiecutter.assembly_name}}.Migrations.System;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

internal interface IMigrationLocator
{
    IEnumerable<Migration> GetMigrations( Direction direction, long version );

    Assembly Assembly { get; }
}

internal class MigrationLocator : IMigrationLocator
{
    private readonly IMigrationActivator _activator;

    public Assembly Assembly { get; }

    public MigrationLocator( string assemblyName, IMigrationActivator activator )
    {
        Assembly = Assembly.Load( new AssemblyName( assemblyName ) );
        _activator = activator ?? throw new ArgumentNullException( nameof( activator ) );
    }

    public MigrationLocator( Assembly assembly, IMigrationActivator activator )
    {
        Assembly = assembly;
        _activator = activator ?? throw new ArgumentNullException( nameof( activator ) );
    }

    public IEnumerable<Migration> GetMigrations( Direction direction, long version )
    {
        static bool MigrationInRange( Type target, MigrationRecord after, MigrationRecord before )
        {
            if ( target == null || !typeof( IMigration ).IsAssignableFrom( target ) || target.IsAbstract )
                return false;

            var attribute = target.GetCustomAttribute<MigrationAttribute>();

            if ( attribute == null )
                return false;

            return after.Version < attribute.Version && attribute.Version <= before.Version;
        }

        // set range according to the direction
        MigrationRecord after;
        MigrationRecord before;

        switch ( direction )
        {
            case Direction.Up:
                after = MigrationRecord.Min();
                before = new MigrationRecord( version );
                break;
            case Direction.Down:
                after = new MigrationRecord( version );
                before = MigrationRecord.Max();
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( direction ), direction, null );
        }

        // get an ordered list of migration instances for the desired range
        var migrations = Assembly
            .GetTypes()
            .Where( type => MigrationInRange( type, after, before ) )
            .Select( type => _activator.CreateInstance( type ) )
            .OrderBy( x => x.GetAttribute().Version, direction )
            .ToList();

        // throw if any duplicates
        var set = new HashSet<long>();

        var duplicate = migrations
            .Select( x => x.GetAttribute().Version )
            .Where( x => !set.Add( x ) )
            .Select( x => new long?( x ) )
            .FirstOrDefault();

        if ( duplicate.HasValue )
            throw new DuplicateMigrationException( $"Migration number conflict detected for version number `{duplicate.Value}`." );

        // done
        return migrations;
    }
}

internal static class LinqExtensions
{
    internal static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>( this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Direction direction ) =>
        direction == Direction.Up
            ? source.OrderBy( keySelector )
            : source.OrderByDescending( keySelector );
}
