using Microsoft.Extensions.Configuration;
using Serilog;

namespace {{cookiecutter.assembly_name }}.Infrastructure.Extensions;

public static class BootstrapExtensions
{
    public static ILogger CreateLogger<T>() where T : class
    {
        var config = CreateConfiguration<T>();

        var loggerConfiguration = new LoggerConfiguration()
            .WithDefaults( config )
            .WithFileWriter( config );

        Log.Logger = loggerConfiguration.CreateBootstrapLogger();
        return Log.ForContext<T>();
    }

    private static IConfiguration CreateConfiguration<T>() where T : class
    {
        return new ConfigurationBuilder() // basic config without cloud secrets
            .SetBasePath( Directory.GetCurrentDirectory() )
            .AddAppSettingsFile()
            .AddAppSettingsEnvironmentFile()
            .AddUserSecrets<T>( optional: true )
            .AddEnvironmentVariables()
            .Build();
    }
}

