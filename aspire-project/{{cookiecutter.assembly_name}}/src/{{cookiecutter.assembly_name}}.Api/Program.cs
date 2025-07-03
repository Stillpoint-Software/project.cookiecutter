
using {{cookiecutter.assembly_name}}.Api.Infrastructure;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using {{cookiecutter.assembly_name}}.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
{% endif %}
using Lamar.Microsoft.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add Lamar 
        LamarSetup.ConfigureLamar(builder);

        {% if cookiecutter.include_azure =="yes" %}
        var connectionString = builder.Configuration["ConnectionStrings:secrets"];

        if (!string.IsNullOrEmpty( connectionString ))
        {
           
            //Add Azure Key Vault secret values to app configuration
            builder.Configuration.AddAzureKeyVaultSecrets( "secrets" );

            //add Azure Key Vault 'SecretClient' to DI Container
            builder.AddAzureKeyVaultClient( "secrets" );
        }

        //Add Azure Blob Storage to DI Container
        builder.AddAzureBlobClient( "blobs" );
        {% endif %}

        // Manually invoke Startup's ConfigureServices
        var startupInstance = new Startup( builder.Configuration );
        startupInstance.ConfigureServices( builder.Services );

        {% if cookiecutter.database == "PostgreSql" %}
        builder.AddNpgsqlDbContext<SampleContext>("{{cookiecutter.database_Name}}");
        {% elif cookiecutter.database == "MongoDb" %}

        builder.AddMongoDBClient("{{cookiecutter.database_Name}}");
        builder.Services.AddScoped<SampleContext>( svc =>
        {
            var scope = svc.CreateScope();
            return SampleContext.Create( scope.ServiceProvider.GetRequiredService<IMongoDatabase>() );
        } );
        {% endif %}

        // Add services to the container before Build()
        builder.Services.AddProblemDetails();

        // Add environment variables
        builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>( optional: true );

        // Configure Serilog setup
        SerilogSetup.ConfigureSerilog( builder );
        {% if cookiecutter.include_audit == "yes" %}
        // Configure audit setup
        AuditSetup.ConfigureAudit( builder );
        {% endif %}
        // Build the application
        var app = builder.Build();

        // Build the application
        app.MapDefaultEndpoints();

        // Call Startup's Configure method to configure the middleware pipeline
        startupInstance.Configure( app, app.Environment );

        // Run the application
        app.Run();
    }
}
