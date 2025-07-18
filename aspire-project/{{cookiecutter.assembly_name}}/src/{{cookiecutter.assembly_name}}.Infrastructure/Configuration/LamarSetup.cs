
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Hyperbee.Pipeline.Context;
using Lamar.Microsoft.DependencyInjection;
using {{cookiecutter.assembly_name}}.Core.Identity;
using {{cookiecutter.assembly_name}}.Core.Validators;
using {{cookiecutter.assembly_name}}.Infrastructure.Data;
using {{cookiecutter.assembly_name}}.Infrastructure.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
{% if cookiecutter.include_audit == "yes" %}
using Audit.Core;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Infrastructure.Configuration;

public static class LamarSetup
{
    public static void ConfigureLamar(IHostBuilder builder)
    {
        builder.UseLamar(registry =>
        {
            registry.Scan(scan =>
            {
                scan.AssemblyContainingType<RegisterServiceAttribute>();

                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
                scan.Convention<RegisterServiceConvention>();
            });

            // Manual helper registrations
            registry.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            registry.AddSingleton<IPrincipalProvider, PrincipalProvider>();
            registry.AddSingleton<IPipelineContextFactory, PipelineContextFactory>();
            registry.AddSingleton<IValidatorProvider, ValidatorProvider>();

            // MVC + JSON settings
            registry.AddControllers()
                    .AddJsonOptions(opts =>
                    {
                        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        opts.JsonSerializerOptions.Converters.Add(new JsonBoolConverter());
                        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    });
        });
    }
}
