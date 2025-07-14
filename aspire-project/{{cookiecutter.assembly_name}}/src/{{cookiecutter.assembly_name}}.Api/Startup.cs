
using Lamar;
using {{ cookiecutter.assembly_name}}.Api.Endpoints;
using {{ cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using {{ cookiecutter.assembly_name}}.ServiceDefaults;
using {{ cookiecutter.assembly_name}}.Infrastructure.Configuration;
using Microsoft.IdentityModel.Logging;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
{% endif %}


namespace {{cookiecutter.assembly_name }}.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(WebApplicationBuilder builder, IServiceCollection services)
    {
        builder.AddServiceDefaults();

        builder.AddBackgroundServices();
        {% if cookiecutter.database == "PostgreSql" %}
        builder.AddNpgsqlDbContext<SampleContext>("{{cookiecutter.database_name}}");
        {% elif cookiecutter.database == "MongoDb" %}
        builder.AddMongoDBClient("{{cookiecutter.database_name}}");
        builder.Services.AddScoped<SampleContext>(svc =>
        {
            var scope = svc.CreateScope();
            return SampleContext.Create(scope.ServiceProvider.GetRequiredService<IMongoDatabase>());
        });
        {% endif %}

        builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>(optional: true);
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        app.MapDefaultEndpoints();
        app.MapMessagingEndpoints();
        app.MapControllers();
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
}