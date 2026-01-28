using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.OpenTelemetry;


namespace {{cookiecutter.assembly_name }}.Infrastructure.Configuration;

public static class SerilogSetup
{
    public static void ConfigureSerilog(IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, lc) => lc
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

                    options.Protocol = OtlpProtocol.HttpProtobuf;

                    var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];

                    foreach (var header in headers)
                    {
                        var (key, value) = header.Split('=') switch
                        {
                            [string k, string v] => (k, v),
                            var v => throw new Exception($"Invalid header format {v}")
                        };

                        options.Headers.Add(key, value);
                    }
                    options.ResourceAttributes.Add("service.name", "apiservice");

                    //To remove the duplicate issue, we can use the below code to get the key and value from the configuration

                    var (otelResourceAttribute, otelResourceAttributeValue) = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
                    {
                        [string k, string v] => (k, v),
                        _ => throw new Exception($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
                    };

                    options.ResourceAttributes.Add(otelResourceAttribute, otelResourceAttributeValue);
                })
                .ReadFrom.Configuration(builder.Configuration)
        );
    }
}
