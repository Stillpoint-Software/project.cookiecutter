using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

public interface IMigrationActivator
{
    Migration CreateInstance( Type migrationType );
}

public class MigrationActivator : IMigrationActivator
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationActivator( IServiceProvider serviceProvider )
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException( nameof( serviceProvider ) );
    }

    public Migration CreateInstance( Type migrationType )
    {
        return (Migration) ActivatorUtilities.CreateInstance( _serviceProvider, migrationType );
    }
}
