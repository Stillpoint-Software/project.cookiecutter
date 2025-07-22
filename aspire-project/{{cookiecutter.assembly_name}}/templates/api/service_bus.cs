public static void AddOpenTelemetry(this IHostApplicationBuilder builder)
{
    builder.AddAzureServiceBusClient(
           "sbemulatorns",
           static settings => settings.DisableTracing = false);

    builder.Services.AddSingleton<ITelemetrySourceProvider, TelemetrySourceProvider>();

    // Fix: Ensure OpenTelemetry.Extensions.Hosting is referenced and use the correct extension method
    builder.Services.AddOpenTelemetry()
        .WithTracing(otel =>
        {
            otel
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("{{cookiecutter.assembly_name}}ServiceBus")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("sbemulatorns"))
                .AddConsoleExporter();
        });
}