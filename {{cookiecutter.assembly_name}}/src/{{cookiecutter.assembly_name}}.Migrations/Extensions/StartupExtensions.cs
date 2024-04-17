using Microsoft.Extensions.Configuration;

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
}

internal static class ConfigurationHelper
{
    internal static string EnvironmentAppSettingsName => $"appsettings.{Environment.GetEnvironmentVariable( "DOTNET_ENVIRONMENT" ) ?? "Development"}.json";
}
