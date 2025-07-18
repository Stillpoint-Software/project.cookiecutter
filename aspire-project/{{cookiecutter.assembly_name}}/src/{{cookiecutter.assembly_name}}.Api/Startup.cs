
using Lamar;
using {{cookiecutter.assembly_name}}.Core.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Infrastructure.Configuration;
using {{cookiecutter.assembly_name}}.Infrastructure.Extensions;
using {{cookiecutter.assembly_name}}.ServiceDefaults;
using Microsoft.IdentityModel.Logging;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
{% endif %}


namespace {{cookiecutter.assembly_name }}.Api;

public class Startup : IStartupRegistry
{

    public void ConfigureServices(IHostApplicationBuilder builder, IServiceCollection services)
    {
        builder.UseStartup<Microservices.Infrastructure.Startup>();

        builder.AddBackgroundServices();
        {% if cookiecutter.database == "PostgreSql" %}
        builder.AddNpgsqlDbContext<DatabaseContext>("{{cookiecutter.database_name}}");
        {% elif cookiecutter.database == "MongoDb" %}
        {% include 'templates/api/api_mongo_service.cs' %}
        {% endif %}
        builder.AddOpenTelemetry();

        builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>(optional: true);
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        app.MapDefaultEndpoints();
        app.MapMessagingEndpoints();
        //app.MapControllers();
    }

    public void ConfigureScanner(ServiceRegistry services)
    {
        IdentityModelEventSource.ShowPII = true; // show pii info in logs for debugging openid

        services.Scan(scanner =>
        {
            scanner.TheCallingAssembly();
            scanner.WithDefaultConventions();
        });
    }
}

public static class StartupExtensions
{
    public static void AddBackgroundServices(this IServiceCollection _)
    {
        /* example
        services.Configure<HeartbeatServiceOptions>( x =>
        {
            x.PeriodSeconds = 10;
            x.Text = "ka-thump";
        } );

        services.AddHostedService<HeartbeatService>();       
         */
    }
    {% if cookiecutter.include_azure_service_bus == "yes" %}
    {% include "templates/api/api_service_bus.cs" %}
    {% endif %}
}