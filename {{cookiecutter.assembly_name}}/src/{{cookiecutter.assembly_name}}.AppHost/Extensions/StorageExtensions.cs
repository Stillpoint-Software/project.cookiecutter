using Aspire.Hosting.Azure;

namespace {{cookiecutter.assembly_name }}.AppHost.Extensions;

public static class StorageExtensions
{
    public static (IResourceBuilder<AzureStorageResource> Storage, IResourceBuilder<AzureBlobStorageResource> Blobs) AddStorageResource(
        this IDistributedApplicationBuilder builder)
    {
        //get's user secrets
        var resourceGroup = builder.Configuration["Parameters:ResourceGroup"];
        var storageConn = builder.Configuration["Parameters:StorageConnection"];
        var storageName = builder.Configuration["Parameters:StorageName"];
        var blobName = builder.Configuration["Parameters:BlobName"];

        IResourceBuilder<AzureStorageResource> storage;

        if (!string.IsNullOrEmpty(resourceGroup) && !string.IsNullOrEmpty(storageConn))
        {
            // Create parameter resources for use with .AsExisting() - check if they exist first
            var existingResourceGroup = builder.GetOrAddParameter("ResourceGroup");
            var existingStorageName = builder.GetOrAddParameter("StorageAccountName");

            storage = builder.AddAzureStorage("storage")
                .AsExisting(existingStorageName, existingResourceGroup);
        }
        else
        {
            storage = builder.AddAzureStorage("storage")
                .RunAsEmulator(azurite =>
                {
                    azurite.WithBlobPort(27000)
                           .WithQueuePort(27001)
                           .WithTablePort(27002)
                           .WithDataBindMount()
                           .WithContainerName("azurite-storage")
                           .WithLifetime(ContainerLifetime.Persistent);
                });
        }

        var blobs = storage.AddBlobs(blobName);
        return (storage, blobs);
    }

}
