var secrets = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureKeyVault("secrets")
    .ConfigureInfrastructure(infra =>
    {
        var keyVault = infra.GetProvisionableResources()
                            .OfType<KeyVaultService>()
                            .Single();

        keyVault.Properties.Sku = new()
        {
            Family = KeyVaultSkuFamily.A,
            Name = KeyVaultSkuName.Standard,
        };
        keyVault.Properties.EnableRbacAuthorization = true;
    })
        : builder.AddConnectionString("secrets");