using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace {{cookiecutter.assembly_name}}.Infrastructure.Configuration;

public static class StartupBuilderExtensions
{
    public static WebApplication ConfigureApplication( this WebApplicationBuilder builder, Action<StartupRegistryCollection> configure )
    {
        var collection = new StartupRegistryCollection( builder.Configuration );
        configure( collection );

        foreach ( var startup in collection.Startups )
        {
            startup.ConfigureServices( builder, builder.Services );
        }

        builder.Host.UseLamar( registry =>
        {
            foreach ( var startup in collection.Startups )
            {
                //startup.ConfigureServices( builder, builder.Services ); //BF try this here
                startup.ConfigureScanner( registry );
            }
        } );

        var app = builder.Build();

        foreach ( var startup in collection.Startups )
        {
            startup.ConfigureApp( app, app.Environment );
        }

        return app;
    }
}

public class StartupRegistryCollection
{
    private readonly IConfiguration _configuration;
    private readonly List<IStartupRegistry> _startups = [];

    internal StartupRegistryCollection( IConfiguration configuration )
    {
        _configuration = configuration;
    }

    public StartupRegistryCollection UseStartup<T>()
        where T : IStartupRegistry
    {
        var instance = (IStartupRegistry) Activator.CreateInstance( typeof(T), _configuration )!;
        _startups.Add( instance );
        return this;
    }

    internal IReadOnlyList<IStartupRegistry> Startups => _startups;
}
