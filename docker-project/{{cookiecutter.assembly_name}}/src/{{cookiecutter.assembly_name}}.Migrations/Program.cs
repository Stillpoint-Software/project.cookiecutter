
using {{ cookiecutter.assembly_name }}.Migrations.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace {{cookiecutter.assembly_name }}.Migrations;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var bootstrapConfig = BootstrapExtensions.CreateBootstrapConfiguration();
        var bootstrapLogger = BootstrapExtensions.CreateBootstrapLogger(bootstrapConfig);

        try
        {
            bootstrapLogger.Information("Starting host...");
            bootstrapLogger.Information($"Using environment settings '{ConfigurationHelper.EnvironmentAppSettingsName}'.");

            await Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    {% if cookiecutter.include_azure == "yes" %}
                    // WARNING: Use the pre-built bootstrapConfig instead of context.Configuration 
                    var vaultName = bootstrapConfig["Azure:KeyVault:VaultName"];
                    {% endif %}
                    builder
                        .AddAppSettingsFile()
                        .AddAppSettingsEnvironmentFile()
                        {% if cookiecutter.include_azure == "yes" %}
                        .AddAzureSecrets(context.HostingEnvironment, vaultName, bootstrapLogger)
                        {% endif %}
                        .AddUserSecrets<Program>(optional: true)
                        .AddEnvironmentVariables()
                        .AddCommandLineEx(args, SwitchMappings());
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<MainService>()
                    {% if cookiecutter.database == "PostgreSql" %}
                        .AddProvider(context.Configuration, bootstrapLogger)
                        .AddMigrations(context.Configuration)
                    {% elif cookiecutter.database == "MongoDb" %}
                        .AddMongoDbProvider(context.Configuration, bootstrapLogger)
                        .AddMongoDbMigrations(context.Configuration)
                    {% endif %}
                        ;
                })
                .UseSerilog()
                .RunConsoleAsync();
        }
        catch (Exception ex)
        {
            bootstrapLogger.Fatal(ex, "Initialization Failure.");
        }
        finally
        {
            bootstrapLogger.Information("Exiting host...");
            await Log.CloseAndFlushAsync();
        }
    }

    private static IDictionary<string, string> SwitchMappings()
    {
        return new Dictionary<string, string>()
        {
            {% if cookiecutter.database == "PostgreSql" %}
            // short names
            { "-f", "[Migrations:FromPaths]" },
            { "-a", "[Migrations:FromAssemblies]" },
            { "-p", "[Migrations:Profiles]" },
            { "-s", "Migrations:SchemaName" },
            { "-t", "Migrations:TableName" },

            { "-cs", "PostgreSql:ConnectionString" },

            // aliases
            { "--file", "[Migrations:FromPaths]" },
            { "--assembly", "[Migrations:FromAssemblies]" },
            { "--profile", "[Migrations:Profiles]" },
            { "--schema", "Migrations:SchemaName" },
            { "--table", "Migrations:TableName" },

            { "--connection", "PostgreSql:ConnectionString" }
{% elif cookiecutter.database == "MongoDb" %}
// short names
{ "-f", "[Migrations:FromPaths]" },
            { "-a", "[Migrations:FromAssemblies]" },
            { "-p", "[Migrations:Profiles]" },
            { "-d", "Migrations:DatabaseName" },
            { "-v", "Migrations:CollectionName" },

            { "-cs", "MongoDb:ConnectionString" },

            // aliases
            { "--file", "[Migrations:FromPaths]" },
            { "--assembly", "[Migrations:FromAssemblies]" },
            { "--profile", "[Migrations:Profiles]" },
            { "--database", "Migrations:DatabaseName" },
            { "--collection", "Migrations:CollectionName" },

            { "--connection", "MongoDb:ConnectionString" }
{% endif %}
        };
    }
}
