using Hyperbee.Extensions.Lamar;
using Hyperbee.Resources;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
{% if cookiecutter.database == "PostgreSql" %}
using Microsoft.EntityFrameworkCore;
{% elif cookiecutter.database == "MongoDb" %}
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
        {% if cookiecutter.database == "PostgreSql" %}
        services.AddSingleton<IDbConnectionProvider, NpgsqlConnectionProvider>( c => new NpgsqlConnectionProvider( Configuration["{{cookiecutter.database}}:ConnectionString"] ) );
        services.AddDbContext<DatabaseContext>( options =>
        {
            options.UseNpgsql( Configuration["{{cookiecutter.database}}:ConnectionString"] );
        } );
        {% elif cookiecutter.database == "MongoDb" %}
        services.AddMongoDb( Configuration );
        services.AddSingleton<IMongoDbService, MongoDbService>();
        {% endif %}
    }

}
