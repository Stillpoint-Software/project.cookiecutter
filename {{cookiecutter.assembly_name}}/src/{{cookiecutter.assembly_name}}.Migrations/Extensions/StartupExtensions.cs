using Microsoft.Extensions.Configuration;
{%- if cookiecutter.database == "PostgreSql" %}
using Hyperbee.Migrations.Providers.Postgres;
{% elif cookiecutter.database == "MongoDb" %}
using Hyperbee.Migrations.Providers.MongoDB;
using MongoDB.Driver;
{%- endif %}
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace {{cookiecutter.assembly_name }}.Migrations.Extensions;

internal static class StartupExtensions
{
    internal static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder)
    {
        return builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }

    internal static IConfigurationBuilder AddAppSettingsEnvironmentFile(this IConfigurationBuilder builder)
    {
        return builder
            .AddJsonFile(ConfigurationHelper.EnvironmentAppSettingsName, optional: true);
    }

{%- if cookiecutter.database =="MongoDb" %}
{% include 'templates/migration/startup_ext.cs' %}
{%- endif %}
}

internal static class ConfigurationHelper
{
    internal static string EnvironmentAppSettingsName => $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json";
}
