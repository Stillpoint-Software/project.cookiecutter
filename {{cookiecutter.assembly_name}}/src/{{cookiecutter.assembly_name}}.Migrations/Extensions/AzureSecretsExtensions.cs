using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace {{cookiecutter.assembly_name}}.Migrations.Extensions;

internal static class AzureSecretsExtensions
{
    internal static IConfigurationBuilder AddAzureSecrets( this IConfigurationBuilder builder, IHostEnvironment hostingEnvironment, string vaultName, ILogger logger )
    {
        logger.Information( $"entering Azure Secrets with params host: {hostingEnvironment.EnvironmentName} vault: {vaultName}" );


        if ( hostingEnvironment.IsDevelopment() )
            return builder;

        if ( string.IsNullOrEmpty( vaultName ) )
            return builder;

        var uri = new Uri( $"https://{vaultName}.vault.azure.net/" );
        builder.AddAzureKeyVault( uri, new DefaultAzureCredential() );
        return builder;
    }
}
