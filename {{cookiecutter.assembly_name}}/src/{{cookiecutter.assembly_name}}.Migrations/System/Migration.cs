using System.Reflection;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public interface IMigration
{
    MigrationAttribute GetAttribute();

    Task UpAsync();

    Task DownAsync();
}

public abstract class Migration : IMigration
{
    public virtual Task UpAsync() => Task.CompletedTask;
    public virtual Task DownAsync() => Task.CompletedTask;

    public MigrationAttribute GetAttribute() => GetType().GetCustomAttribute<MigrationAttribute>();
}
