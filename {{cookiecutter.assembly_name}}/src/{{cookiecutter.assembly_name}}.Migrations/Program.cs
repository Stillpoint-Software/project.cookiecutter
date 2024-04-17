using {{cookiecutter.assembly_name}}.Migrations.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace {{cookiecutter.assembly_name}}.Migrations;

internal class Program
{
    public static async Task Main( string[] args )
    {
        var bootstrapConfig = BootstrapExtensions.CreateBootstrapConfiguration();
        var bootstrapLogger = BootstrapExtensions.CreateBootstrapLogger( bootstrapConfig );

        try
        {
            bootstrapLogger.Information( "Starting host..." );
            bootstrapLogger.Information( $"Using environment settings '{ConfigurationHelper.EnvironmentAppSettingsName}'." );

            await Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration( ( context, builder ) =>
                {
                    // WARNING: Use the pre-built bootstrapConfig instead of context.Configuration 
                    var vaultName = bootstrapConfig["Azure:KeyVault:VaultName"];

                    builder
                        .AddAppSettingsFile()
                        .AddAppSettingsEnvironmentFile()
                        .AddAzureSecrets( context.HostingEnvironment, vaultName, bootstrapLogger )
                        .AddUserSecrets<Program>( optional: true )
                        .AddEnvironmentVariables()
                        .AddCommandLineEx( args, SwitchMappings() );
                } )
                .ConfigureServices( ( context, services ) =>
                {
                    services
                        .AddHostedService<MainService>();
                } )
                .UseSerilog()
                .RunConsoleAsync();
        }
        catch ( Exception ex )
        {
            bootstrapLogger.Fatal( ex, "Initialization Failure." );
        }
        finally
        {
            bootstrapLogger.Information( "Exiting host..." );
            await Log.CloseAndFlushAsync();
        }
    }

    private static IDictionary<string, string> SwitchMappings()
    {
        return new Dictionary<string, string>()
        {
            // short names
            { "-c", "{{cookiecutter.database}}:ConnectionString" },
            { "-r", "Runner:HardReset" },

            // aliases
            { "--connection", "{{cookiecutter.database}}:ConnectionString" },
            { "--reset", "Runner:HardReset" },
        };
    }
}
