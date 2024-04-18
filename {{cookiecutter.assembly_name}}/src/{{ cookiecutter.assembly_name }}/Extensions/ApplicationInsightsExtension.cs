using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;


namespace {{cookiecutter.assembly_name}}.Extensions;

public interface ITelemetryClientProvider
{
    public TelemetryClient Client { get; }
}

public class TelemetryClientProvider : ITelemetryClientProvider
{
    public required TelemetryClient Client { get; init; }
}


public static class ApplicationInsightsExtensions
{
    public static void AddApplicationInsights( this IServiceCollection services, IConfiguration config )
    {
        // use overload that takes our IConfiguration instance,
        // otherwise we won't be able to use user secrets

        services.AddApplicationInsightsTelemetry( config );

        services.AddSingleton<ITelemetryClientProvider>(
            c =>
            {
                var telemetryConfig = c.GetRequiredService<TelemetryConfiguration>();
                var telemetryClient = new TelemetryClient( telemetryConfig );

                return new TelemetryClientProvider
                {
                    Client = telemetryClient
                };
            }
        );
    }
}