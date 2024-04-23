using System.Runtime.Serialization;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public class MigrationException : Exception
{
    public MigrationException()
        : base( "Migration exception." )
    {
    }

    public MigrationException( string message )
        : base( message )
    {
    }

    public MigrationException( string message, Exception innerException )
        : base( message, innerException )
    {
    }

    public MigrationException( SerializationInfo info, StreamingContext context )
        : base( info, context )
    {
    }
}
