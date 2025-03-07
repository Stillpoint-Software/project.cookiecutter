
using {{cookiecutter.assembly_name}}.Api.Infrastructure;
using {{cookiecutter.assembly_name}}.Data.Postgres;
using {{cookiecutter.assembly_name}}.ServiceDefaults;

namespace {{cookiecutter.assembly_name}}.Api;

public class Program
{
    public static void Main( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add services to the container.
        LamarSetup.ConfigureLamar( builder );

        var connectionString = builder.Configuration["ConnectionStrings:secrets"];

        if (!string.IsNullOrEmpty( connectionString ))
        {
            //Add Azure Key Vault secret values to app configuration
            builder.Configuration.AddAzureKeyVaultSecrets( "secrets" );

            //add Azure Key Vault 'SecretClient' to DI Container
            builder.AddAzureKeyVaultClient( "secrets" );
        }

        // Manually invoke Startup's ConfigureServices
        var startupInstance = new Startup( builder.Configuration );
        startupInstance.ConfigureServices( builder.Services );

        // Add database context
        builder.AddNpgsqlDbContext<ProjectContext>( "projectdb" );

        // Add environment variables
        builder.Configuration
            .AddEnvironmentVariables();

        // Configure Serilog setup
        SerilogSetup.ConfigureSerilog( builder );

        // Configure audit setup
        AuditSetup.ConfigureAudit( builder );

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
