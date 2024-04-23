namespace {{cookiecutter.assembly_name}}.Migrations.System;

public class MigrationAttribute : Attribute
{
    public MigrationAttribute( long version, string description = null )
    {
        Version = version;
        Description = description;
    }

    public long Version { get; }

    public string Description { get; }
}
