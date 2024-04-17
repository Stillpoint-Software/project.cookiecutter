namespace {{cookiecutter.assembly_name}}.Extensions;
public static class EnvironmentHelper
{
    public static string EnvironmentName =>
        Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ) ?? "Development";

    public static string EnvironmentAppSettingsName => $"appsettings.{EnvironmentName}.json";
}

public static class SettingsExtensions
{
    public static IConfigurationBuilder AddAppSettingsFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );
    }

    public static IConfigurationBuilder AddAppSettingsEnvironmentFile( this IConfigurationBuilder builder )
    {
        return builder
            .AddJsonFile( EnvironmentHelper.EnvironmentAppSettingsName, optional: true );
    }
}
