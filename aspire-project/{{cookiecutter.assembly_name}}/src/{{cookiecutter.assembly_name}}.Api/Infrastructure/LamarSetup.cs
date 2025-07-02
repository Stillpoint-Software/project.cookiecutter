{% if cookiecutter.include_audit == "yes" %}
using Audit.Core;
{% endif %} 
using Hyperbee.Pipeline.Context;
using Lamar.Microsoft.DependencyInjection;
using {{ cookiecutter.assembly_name}}.Api.Identity;
using {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;
using {{ cookiecutter.assembly_name}}.Api.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;


namespace {{cookiecutter.assembly_name}}.Api.Infrastructure;

public class LamarSetup
{
    public static void ConfigureLamar( WebApplicationBuilder builder )
    {
        builder.Host.UseLamar( ( registry ) =>
        {
            //register services using Lamar
            registry.AddSingleton<ISampleService, SampleService>();
            {% if cookiecutter.include_audit == "yes" %}
            registry.AddSingleton<IAuditScopeFactory, AuditScopeFactory>();
            {% endif %}
            registry.AddSingleton<ICreateSampleCommand, CreateSampleCommand>();
            registry.AddSingleton<IGetSampleCommand, GetSampleCommand>();
            registry.AddSingleton<IUpdateSampleCommand, UpdateSampleCommand>();
            registry.AddSingleton<IPipelineContextFactory, PipelineContextFactory>();
            registry.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            registry.AddSingleton<IPrincipalProvider, PrincipalProvider>();
            registry.AddSingleton<IValidatorProvider, ValidatorProvider>();

            registry.Scan(scan =>
          {
              scan.AssemblyContainingType<SampleValidation>();
              scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
          });

            // discover MVC controllers -- this was problematic
            // inside of the UseLamar() method, but is "fixed" in
            // Lamar V8
            registry.AddControllers()
            .AddJsonOptions( x =>
            {
                // Serialize enums as strings in API responses (e.g., Color)
                x.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() );
                x.JsonSerializerOptions.Converters.Add( new JsonBoolConverter() );
                x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            } );
        } );
    }
}
