using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public class MigrationRecord
{
    public static MigrationRecord Min() => new( -1 );

    public static MigrationRecord Max() => new( long.MaxValue );

    public Guid Id { get; } = Guid.NewGuid();

    public long Version { get; init; }

    public string Description { get; init; }

    [BsonRepresentation( BsonType.String )]
    public DateTimeOffset RunOn { get; init; } = DateTimeOffset.UtcNow;

    public MigrationRecord()
    {
    }

    public MigrationRecord( long version, string description = null )
    {
        Version = version;
        Description = description;
    }

    public MigrationRecord( IMigration migration )
    {
        if ( migration == null )
            throw new ArgumentNullException( nameof( migration ) );

        var name = migration.GetType().Name;

        var attribute = migration.GetAttribute();

        if ( attribute == null )
            throw new NotSupportedException( $"Missing {nameof( MigrationAttribute )} for migration type {name}." );

        Version = attribute.Version;
        Description = !string.IsNullOrWhiteSpace( attribute.Description ) ? attribute.Description : name;
    }

    public override string ToString()
    {
        return $"[{Version}] {Description}";
    }
}
