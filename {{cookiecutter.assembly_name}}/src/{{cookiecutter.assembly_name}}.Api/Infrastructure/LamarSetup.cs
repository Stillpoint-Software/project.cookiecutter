using Audit.Core;
using Hyperbee.Pipeline.Context;
using Lamar.Microsoft.DependencyInjection;
using {{cookiecutter.assembly_name}}.Api.Commands.Patient;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Postgres.Services;
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
            registry.AddSingleton<IAuditScopeFactory, AuditScopeFactory>();
            registry.AddSingleton<IGetAllPatientsCommand, GetAllPatientCommand>();
            registry.AddSingleton<IGetPatientByFilterCommand, GetPatientByFilterCommand>();
            registry.AddSingleton<IGetPatientByIdCommand, GetPatientByIdCommand>();
            registry.AddSingleton<ICreatePatientCommand, CreatePatientCommand>();
            registry.AddSingleton<IUpdatePatientCommand, UpdatePatientCommand>();
            registry.AddSingleton<IDeletePatientCommand, DeletePatientCommand>();
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
