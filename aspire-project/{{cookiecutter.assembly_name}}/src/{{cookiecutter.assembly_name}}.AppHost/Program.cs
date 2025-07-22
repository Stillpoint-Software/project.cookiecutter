using {{cookiecutter.assembly_name}}.AppHost;
using Microsoft.Extensions.Hosting;
{% if cookiecutter.include_azure_key_vault == "yes" %}
using Azure.Provisioning.KeyVault;
{% endif %}
{% if cookiecutter.include_azure_storage == "yes" %}
using Azure.Provisioning.Storage;
{% endif %}

var builder = DistributedApplication.CreateBuilder(args);

{% if cookiecutter.include_azure_key_vault == "yes" %}
{% include 'templates/host/key_vault.cs' %}
{% endif %}

{% if cookiecutter.include_azure_application_insights == "yes" %}
{% include 'templates/host/application_insights.cs' %}
{% endif %}

{% if cookiecutter.include_azure_storage == "yes" %}
{% include 'templates/host/storage.cs' %}
{% endif %}

{% if cookiecutter.include_azure_service_bus == "yes" %}

{% include 'templates/host/service_bus.cs' %}
{% endif %}

{% if cookiecutter.database == "PostgreSql" %}
{% include 'templates/host/postgresql.cs' %}
{% elif cookiecutter.database == "MongoDb" %}
{% include 'templates/host/mongodb.cs' %}
{% endif %}

var projectdb = dbServer.AddDatabase("{{cookiecutter.database_name}}");

var apiService = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Api>("{{cookiecutter.assembly_name|lower }}-api")
    .WithReference(projectdb)
    .WithExternalHttpEndpoints()
    {% if cookiecutter.include_azure_key_vault == "yes" %}
    .WithReference(secrets)
    {% endif %}
    {% if cookiecutter.include_azure_application_insights == "yes" %}
    .WithReference(appInsights)
    {% endif %}
    {% if cookiecutter.include_azure_service_bus == "yes" %}
    .WithReference( serviceBus ).WaitFor( serviceBus )
    {% endif %}
    .WithSwaggerUI()
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Migrations>("{{cookiecutter.assembly_name|lower }}-migrations")
    .WaitFor(projectdb)
     {% if cookiecutter.include_azure_application_insights == "yes" %}
     .WithReference( appInsights )
     {% endif %}
    .WithReference( projectdb );  

builder.Build().Run();