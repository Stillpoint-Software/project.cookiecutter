using Azure.Identity;

namespace {{cookiecutter.assembly_name}}.Extensions;

public static class AzureSecretsExtensions
{
    {%- if cookiecutter.include_azure_key_vault == "yes" -%}
    public static IConfigurationBuilder AddAzureSecrets( this IConfigurationBuilder builder, IHostEnvironment hostingEnvironment, string vaultName )
    {
        if ( hostingEnvironment.IsDevelopment() )
            return builder;

        if ( string.IsNullOrEmpty( vaultName ) )
            return builder;

        var uri = new Uri( $"https://{vaultName}.vault.azure.net/" );
        builder.AddAzureKeyVault( uri, new DefaultAzureCredential() );
        return builder;
    }
    {% endif %}
}
