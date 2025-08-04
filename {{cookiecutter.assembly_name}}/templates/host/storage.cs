var storage = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureStorage("storage").ConfigureInfrastructure(infra =>
{
    var storageAccount = infra.GetProvisionableResources()
                              .OfType<StorageAccount>()
                              .Single();

    storageAccount.AccessTier = StorageAccountAccessTier.Cool;
    storageAccount.Sku = new StorageSku { Name = StorageSkuName.StandardLrs };
}) : builder.AddAzureStorage("storage").RunAsEmulator(az =>
{
    az.WithDataBindMount();
});