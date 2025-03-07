
using {{ cookiecutter.assembly_name}}.Extensions;
using Lamar.Microsoft.DependencyInjection;
using Serilog;

namespace {{ cookiecutter.assembly_name}};

 {% if cookiecutter.include_azure == "yes" %}
   {% include '/templates/azure/main_program.cs' % }
{% else %}
internal class Program
{
    public static async Task<int> Main( string[] args )    {
        var bootstrapConfig = BootstrapExtensions.CreateBootstrapConfiguration<Program>(); // basic config without cloud secrets
        var bootstrapLogger = BootstrapExtensions.CreateBootstrapLogger<Program>( bootstrapConfig );

        try
        {
            bootstrapLogger.Information( "Starting host..." );
            bootstrapLogger.Information( $"Using environment settings '{EnvironmentHelper.EnvironmentAppSettingsName}'." );

            // build host
            var host = Host.CreateDefaultBuilder( args )
                .UseLamar()
                .UseSerilog( ( context, services, builder ) =>
                {
                    builder
                        .WithDefaults( context.Configuration )
                        .WithConsole()
                        .WithFileWriter( context.Configuration );
                .ConfigureWebHostDefaults( builder =>
                {
                    builder
                        .UseStartup<Startup>();
                } )
                .ConfigureAppConfiguration( ( context, builder ) =>
                {
                    builder
                        .AddAppSettingsFile()
                        .AddAppSettingsEnvironmentFile()
                        .AddUserSecrets<Program>( optional: true ) // secrets won't exist in non-local environments
                        .AddEnvironmentVariables();
                } )
                .Build();

            // run
            await host.RunAsync();
        }
        catch ( Exception ex )
        {
            bootstrapLogger.Fatal( ex, "Host terminated unexpectedly." );
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }

        return 0;
    }
}
{% endif %}