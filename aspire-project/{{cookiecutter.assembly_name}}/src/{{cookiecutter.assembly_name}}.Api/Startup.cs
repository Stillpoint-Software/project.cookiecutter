
#define CONTAINER_DIAGNOSTICS
using FluentValidation;
using Lamar;
using Microsoft.IdentityModel.Logging;
using {{ cookiecutter.assembly_name }}.Api.Commands.SampleArea;
using {{ cookiecutter.assembly_name }}.Api.Endpoints;
using {{ cookiecutter.assembly_name }}.Api.Validators;
using {{ cookiecutter.assembly_name }}.Core.Identity;
using {{ cookiecutter.assembly_name }}.Core.Validators;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Services;
using {{ cookiecutter.assembly_name }}.Data.{{ cookiecutter.database }};
using {{ cookiecutter.assembly_name }}.Data.PostgreSql.Services;
using {{ cookiecutter.assembly_name }}.Infrastructure.Configuration;
using {{ cookiecutter.assembly_name }}.Infrastructure.Extensions;
using {{ cookiecutter.assembly_name }}.ServiceDefaults;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
{% endif %}


namespace {{cookiecutter.assembly_name }}.Api;

public class Startup : IStartupRegistry
{

    public void ConfigureServices(IHostApplicationBuilder builder, IServiceCollection services)
    {
        builder.UseStartup <{{ cookiecutter.assembly_name }}.Infrastructure.Startup > ();

        builder.AddBackgroundServices();
        {% if cookiecutter.database == "PostgreSql" %}
        builder.AddNpgsqlDbContext<DatabaseContext>("{{cookiecutter.database_name}}");
        {% elif cookiecutter.database == "MongoDb" %}
        {% include 'templates/api/mongo_service.cs' %}
        {% endif %}
        {% if cookiecutter.include_azure_service_bus == "yes" %}
        builder.AddOpenTelemetry();
        {% endif %}

        builder.Configuration
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>(optional: true);
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        app.MapDefaultEndpoints();
        app.MapSampleEndpoints();
        //app.MapControllers();
    }

    public void ConfigureScanner(ServiceRegistry services)
    {
        IdentityModelEventSource.ShowPII = true; // show pii info in logs for debugging openid

        services.Scan(scanner =>
        {
            scanner.AssemblyContainingType<SampleValidation>();
            scanner.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
            scanner.TheCallingAssembly();
            scanner.WithDefaultConventions();
        });

        services.For<ISampleService>().Use<SampleService>();
        services.For<IPrincipalProvider>().Use<PrincipalProvider>();
        services.For<ICreateSampleCommand>().Use<CreateSampleCommand>();
        services.For<IValidatorProvider>().Use<ValidatorProvider>();
    }
    private static void ContainerDiagnostics(IApplicationBuilder app, IHostEnvironment env)
    {
#if CONTAINER_DIAGNOSTICS
        if (!env.IsDevelopment())
            return;

        var container = (IContainer)app.ApplicationServices;
        Console.WriteLine(container.WhatDidIScan());
        Console.WriteLine(container.WhatDoIHave());
#endif
    }
}

public static class StartupExtensions
{
    public static void AddBackgroundServices(this IHostApplicationBuilder builder)
    {
        /* example
        services.Configure<HeartbeatServiceOptions>( x =>
        {
            x.PeriodSeconds = 10;
            x.Text = "ka-thump";
        } );

        services.AddHostedService<HeartbeatService>();       
         */
    }
    {% if cookiecutter.include_azure_service_bus == "yes" %}
    {% include "templates/api/service_bus.cs" %}
{% endif %}
}