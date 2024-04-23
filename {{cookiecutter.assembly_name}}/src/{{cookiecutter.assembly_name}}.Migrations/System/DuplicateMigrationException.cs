using System.Runtime.Serialization;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public class DuplicateMigrationException : Exception
{
    public DuplicateMigrationException()
        : base( "Duplicate migration exception." )
    {
    }

    public DuplicateMigrationException( string message )
        : base( message )
    {
    }

    public DuplicateMigrationException( string message, Exception innerException )
        : base( message, innerException )
    {
    }

    public DuplicateMigrationException( SerializationInfo info, StreamingContext context )
        : base( info, context )
    {
    }
}
