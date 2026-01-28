using Aspire.Hosting.Azure;
using AzureKeyVaultEmulator.Aspire.Hosting;

namespace {{cookiecutter.assembly_name }}.AppHost.Extensions;


public static class KeyVaultExtensions
{
    public static IResourceBuilder<AzureKeyVaultResource> AddKeyVaultResource(
        this IDistributedApplicationBuilder builder)
    {
        //get's user secrets
        var resourceGroup = builder.Configuration["Parameters:ResourceGroup"];
        var keyVaultConn = builder.Configuration["Parameters:KeyVaultConnection"];
        var keyVaultName = builder.Configuration["Parameters:KeyVaultName"];

        if (!string.IsNullOrEmpty(resourceGroup) &&
            !string.IsNullOrEmpty(keyVaultConn) &&
            !string.IsNullOrEmpty(keyVaultName))
        {
            // Create parameter resources for use with .AsExisting() - check if they exist first
            var existingResourceGroup = builder.GetOrAddParameter("ResourceGroup");
            var existingKeyVaultName = builder.GetOrAddParameter("KeyVaultName");

            return builder.AddAzureKeyVault("keyVault")
                .AsExisting(existingKeyVaultName, existingResourceGroup);
        }

        return builder.AddAzureKeyVault("keyVault")
            .RunAsEmulator(new KeyVaultEmulatorOptions { Persist = true });
    }
}
