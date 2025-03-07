   public static LoggerConfiguration WithAzureApplicationInsights( this LoggerConfiguration loggerConfiguration, IServiceProvider services )
    {
        // https://github.com/serilog-contrib/serilog-sinks-applicationinsights
        //
        // DI requires a configuration connection string "ApplicationInsights:ConnectionString" or
        // environment variable APPLICATIONINSIGHTS_CONNECTION_STRING
        var telemetryClient = services.GetRequiredService<ITelemetryClientProvider>().Client;
        loggerConfiguration.WriteTo.ApplicationInsights( telemetryClient, TelemetryConverter.Traces );

        return loggerConfiguration;
    }