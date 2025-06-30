using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace {{cookiecutter.assembly_name}}.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration WithDefaults( this LoggerConfiguration loggerConfiguration,
        IConfiguration config )
    {
        loggerConfiguration
            .MinimumLevel.Debug()
            .ReadFrom.Configuration( config )
            .Enrich.FromLogContext();

        return loggerConfiguration;
    }

    public static LoggerConfiguration WithConsole( this LoggerConfiguration loggerConfiguration,
        LogEventLevel minimumConsoleLevel = LogEventLevel.Error )
    {
        loggerConfiguration
            .WriteTo.Console( restrictedToMinimumLevel: minimumConsoleLevel );

        return loggerConfiguration;
    }

    public static LoggerConfiguration WithFileWriter( this LoggerConfiguration loggerConfiguration,
        IConfiguration config )
    {
        var appName = config["Api:AppName"] ?? "{{cookiecutter.assembly_name}}";
        var jsonFormatter = new CompactJsonFormatter();
        var pathFormat = $".{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}{appName}-.json";

        loggerConfiguration.WriteTo.File( jsonFormatter, pathFormat, rollingInterval: RollingInterval.Day, shared: true );

        return loggerConfiguration;
    }

  {% if cookiecutter.include_azure == "yes" %}
  {% include "/templates/main/main_extensions_log.cs"%}
  {% endif %}
}
