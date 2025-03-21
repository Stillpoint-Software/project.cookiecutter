        var keyVaultNameSpace = config["MongoDb:KeyVaultNamespace"];
          services.AddSingleton<IMongoDbService, MongoDbService>(
            c => new MongoDbService( connectionString, databaseName, keyVaultNameSpace )
        );