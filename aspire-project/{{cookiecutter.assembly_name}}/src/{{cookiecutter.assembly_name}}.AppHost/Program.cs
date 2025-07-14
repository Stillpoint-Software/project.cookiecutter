using {{cookiecutter.assembly_name}}.AppHost;
using Microsoft.Extensions.Hosting;
{% if cookiecutter.include_azure == "yes" %}
using Azure.Provisioning.KeyVault;
using Azure.Provisioning.Storage;
{% endif %}

var builder = DistributedApplication.CreateBuilder(args);

{% if cookiecutter.include_azure == "yes" %}
// Conditionally update the app model with secrets.
// Automatically provision Key Vault in Azure or use local secrets
var secrets = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureKeyVault( "secrets" )
    .ConfigureInfrastructure( infra =>
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
    } )
        : builder.AddConnectionString( "secrets" );

// Automatically provision an Application Insights resource
var appInsights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights( "appInsights" )
    : builder.AddConnectionString( "appInsights", "APPLICATIONINSIGHTS_CONNECTION_STRING" );

//Azure Storage
//Azure Storage Deployment
var storage = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureStorage( "storage" ).ConfigureInfrastructure( infra =>
{
    var storageAccount = infra.GetProvisionableResources()
                              .OfType<StorageAccount>()
                              .Single();

    storageAccount.AccessTier = StorageAccountAccessTier.Cool;
    storageAccount.Sku = new StorageSku { Name = StorageSkuName.StandardLrs };
} ) : builder.AddAzureStorage( "storage" ).RunAsEmulator( az =>
{
    az.WithDataBindMount();
} );
{% endif %}

{% if cookiecutter.include_service_bus == "yes" %}
//Azure Service Bus
var projectdb = dbServer.AddDatabase("samplemessages");


var serviceBus = builder.AddAzureServiceBus("sbemulatorns").RunAsEmulator(emulator =>
{
    emulator.WithHostPort(7777);
});

var topic = serviceBus.AddServiceBusTopic("topic");
topic.AddServiceBusSubscription("sub2");
{% endif %}

{% if cookiecutter.database == "PostgreSql" %}
var dbPassword = builder.AddParameter("DbPassword", "postgres", true);
var dbUser = builder.AddParameter("DbUser", "postgres", true);

var dbServer = builder.AddPostgres("postgres", userName: dbUser, password: dbPassword)
    .PublishAsConnectionString()
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("9.5"));

{% elif cookiecutter.database == "MongoDb" %}
var dbUsername = builder.AddParameter( "DbUser", "mongodb", true );
var dbPassword = builder.AddParameter( "DbPassword","mongodb", secret: true );

var dbServer = builder.AddMongoDB( "mongo", userName: dbUsername, password: dbPassword )
                .WithMongoExpress()
                .PublishAsConnectionString()
                .WithDataVolume();

if ( builder.Environment.IsDevelopment() )
{
  dbServer.WithLifetime( ContainerLifetime.Persistent );
}                
{% endif %}

var projectdb = dbServer.AddDatabase("{{cookiecutter.database_name}}");

var apiService = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Api>("{{cookiecutter.assembly_name|lower }}-api")
    .WithReference(projectdb)
    .WithExternalHttpEndpoints()
    {% if cookiecutter.include_azure == "yes" %}
    .WithReference( secrets )
    .WithReference( appInsights )
    {% endif %}
    {% if cookiecutter.include_service_bus == "yes" %}
    .WithReference( serviceBus ).WaitFor( serviceBus )
    {% endif %}
    .WithSwaggerUI()
    .WithHttpHealthCheck();

builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Migrations>("{{cookiecutter.assembly_name|lower }}-migrations")
    .WaitFor(projectdb)
     {% if cookiecutter.include_azure == "yes" %}
     .WithReference( appInsights )
     {% endif %}
    .WithReference( projectdb );  

builder.Build().Run();