using {{cookiecutter.assembly_name}}.Infrastructure.Configuration;
using {{cookiecutter.assembly_name}}.Infrastructure.Extensions;
using Serilog;

namespace {{cookiecutter.assembly_name}}.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = BootstrapExtensions.CreateLogger<Program>();

        try
        {
            logger.Information("Starting host...");
            logger.Information("Using environment settings '{EnvironmentAppSettingsName}'.", ConfigurationHelper.EnvironmentAppSettingsName);

            var builder = WebApplication.CreateBuilder(args);

            var app = builder.ConfigureApplication(configure =>
            {
                configure.UseStartup<Startup>();
                configure.UseStartup<Infrastructure.Startup>();
            });

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Host terminated unexpectedly.");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }

        return 0;
    }
}