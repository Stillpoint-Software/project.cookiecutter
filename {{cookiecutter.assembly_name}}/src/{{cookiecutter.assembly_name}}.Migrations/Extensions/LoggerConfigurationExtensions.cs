using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace {{cookiecutter.assembly_name}}.Migrations.Extensions;

internal static class LoggerConfigurationExtensions
{
    internal static LoggerConfiguration WithDefaults( this LoggerConfiguration loggerConfiguration,
        IConfiguration config )
    {
        loggerConfiguration
            .MinimumLevel.Debug()
            .ReadFrom.Configuration( config )
            .Enrich.FromLogContext()
            .WriteTo.Console( restrictedToMinimumLevel: LogEventLevel.Debug );

        return loggerConfiguration;
    }

    internal static LoggerConfiguration WithFileWriter( this LoggerConfiguration loggerConfiguration,
        IConfiguration config )
    {
        var appName = config["Api:AppName"] ?? "{{cookiecutter.assembly_name}}";
        var jsonFormatter = new CompactJsonFormatter();
        var pathFormat = $".{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}{appName}-.json";

        loggerConfiguration.WriteTo.File( jsonFormatter, pathFormat, rollingInterval: RollingInterval.Day,
            shared: true );

        return loggerConfiguration;
    }
}
