using Microsoft.Extensions.Configuration;
using Serilog;
using ILogger = Serilog.ILogger;

namespace {{cookiecutter.assembly_name}}.Migrations.Extensions;

public static class BootstrapExtensions
{
    public static IConfiguration CreateBootstrapConfiguration()
    {
        return new ConfigurationBuilder() // basic config without cloud secrets
            .SetBasePath( Directory.GetCurrentDirectory() )
            .AddAppSettingsFile()
            .AddUserSecrets<Program>( optional: true )
            .AddAppSettingsEnvironmentFile()
            .AddEnvironmentVariables()
            .Build();
    }

    public static ILogger CreateBootstrapLogger( IConfiguration config )
    {
        var loggerConfiguration = new LoggerConfiguration()
            .WithDefaults( config )
            .WithFileWriter( config );

        Log.Logger = loggerConfiguration.CreateBootstrapLogger();
        return Log.ForContext<Program>();
    }
}
