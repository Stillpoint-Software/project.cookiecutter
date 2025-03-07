using Microsoft.Extensions.Configuration;
{% if cookiecutter.database == "PostgreSql" %}
using Hyperbee.Migrations.Providers.Postgres;
{% elif cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Migrations.Extensions;

internal static class StartupExtensions
{
    internal static IConfigurationBuilder AddAppSettingsFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );
    }

    internal static IConfigurationBuilder AddAppSettingsEnvironmentFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( ConfigurationHelper.EnvironmentAppSettingsName, optional: true );
    }

{% if cookiecutter.database == "PostgreSql" %}
    {% include '/templates/data/data_startup_ext_postgresql.cs' %}
{% endif %}

{% if cookiecutter.database == "MongoDb" %}
    {% include '/templates/data/data_startup_ext_mongodb.cs' %}
{% endif %}
}

internal static class ConfigurationHelper
{
    internal static string EnvironmentAppSettingsName => $"appsettings.{Environment.GetEnvironmentVariable( "DOTNET_ENVIRONMENT" ) ?? "Development"}.json";
}
