using Audit.Core;
using Hyperbee.Pipeline.Context;
using Lamar.Microsoft.DependencyInjection;
using {{cookiecutter.assembly_name}}.Api.Commands.Patient;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
{% if cookiecutter.database == "PostgreSql" %}
using {{cookiecutter.assembly_name}}.Data.Postgres.Services;
{% elif cookiecutter.database == "MongoDb" %}
//TODO
{% endif %}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace {{cookiecutter.assembly_name}}.Api.Infrastucture;

public class LamarSetup
{
    public static void ConfigureLamar( WebApplicationBuilder builder )
    {
        builder.Host.UseLamar( ( context, registry ) =>
        {
            //register services using Lamar
            registry.AddSingleton<IPatientService, PatientService>();
            {% if cookiecutter.use_audit == "yes" %}
            registry.AddSingleton<IAuditScopeFactory, AuditScopeFactory>();
            {% endif %}
            registry.AddSingleton<ICreateSampleCommand, CreateSampleCommand>();
            registry.AddSingleton<IGetSampleCommand, GetSampleCommand>();
            registry.AddSingleton<IUpdateSampleCommand, UpdateSampleCommand>();
            registry.AddSingleton<IPipelineContextFactory, PipelineContextFactory>();

            // Add your own Lamar ServiceRegistry collections
            // of registrations
            //registry.IncludeRegistry<MyRegistry>();

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
