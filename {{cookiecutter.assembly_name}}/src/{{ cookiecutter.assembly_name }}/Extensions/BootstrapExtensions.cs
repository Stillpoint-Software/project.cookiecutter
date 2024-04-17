using Serilog;
using ILogger = Serilog.ILogger;

namespace {{cookiecutter.assembly_name}}.Extensions;
public static class BootstrapExtensions
{
    public static IConfiguration CreateBootstrapConfiguration<T>() where T : class
    {
        return new ConfigurationBuilder() // basic config without cloud secrets
            .SetBasePath( Directory.GetCurrentDirectory() )
            .AddAppSettingsFile()
            .AddAppSettingsEnvironmentFile()
            .AddUserSecrets<T>( optional: true )
            .AddEnvironmentVariables()
            .Build();
    }

    public static ILogger CreateBootstrapLogger<T>( IConfiguration config ) where T : class
    {
        var loggerConfiguration = new LoggerConfiguration()
            .WithDefaults( config )
            .WithConsole()
            .WithFileWriter( config );

        Log.Logger = loggerConfiguration.CreateBootstrapLogger();
        return Log.ForContext<T>();
    }
}
