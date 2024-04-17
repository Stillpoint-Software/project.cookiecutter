using Hyperbee.Extensions.Lamar;
using Hyperbee.Resources;
using Lamar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

public class Startup( IConfiguration configuration ) : IStartupRegistry
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureContainer( ServiceRegistry services )
    {
        // auto-registrations by convention

        services.Scan( _ =>
        {
            _.TheCallingAssembly();
            _.WithDefaultConventions();
        } );

        // explicit registrations

        services.AddSingleton( typeof( IResourceProvider<> ), typeof( ResourceProvider<> ) );
        services.AddSingleton<IDbConnectionProvider, NpgsqlConnectionProvider>( c => new NpgsqlConnectionProvider( Configuration["{{cookiecutter.database}}:ConnectionString"] ) );
        services.AddDbContext<SampleContext>( options =>
        {
            options.UseNpgsql( Configuration["{{cookiecutter.database}}:ConnectionString"] );
        } );

        services.AddHealthChecks()
            .AddDbContextCheck<SampleContext>();
    }
}
