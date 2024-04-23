using Hyperbee.Extensions.Lamar;
using Hyperbee.Resources;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
{%- if cookiecutter.database == "Postgresql" -%}
using Microsoft.EntityFrameworkCore;
{%- elif cookiecutter.database == "Mongo" -%}
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Extensions;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;
{% endif %}
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
        {%- if cookiecutter.database == "Postgresql" -%}
        services.AddSingleton<IDbConnectionProvider, NpgsqlConnectionProvider>( c => new NpgsqlConnectionProvider( Configuration["{{cookiecutter.database}}:ConnectionString"] ) );
        services.AddDbContext<SampleContext>( options =>
        {
            options.UseNpgsql( Configuration["{{cookiecutter.database}}:ConnectionString"] );
        } );

        services.AddHealthChecks()
            .AddDbContextCheck<SampleContext>();
        
        {%- elif cookiecutter.database == "Mongo" -%}
         services.AddMongoDb( Configuration );
        services.AddSingleton<IMongoDbService, MongoDbService>();

        services.AddHealthChecks()
           .AddMongoDb( Configuration["MongoDb:ConnectionString"], "MongoDb Health", HealthStatus.Degraded );
        {% endif %}
    }

}
