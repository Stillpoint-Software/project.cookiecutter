
using {{ cookiecutter.assembly_name}}.AppHost.Extensions;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

{% if cookiecutter.include_oauth %}
//get's user secrets
var issuer = builder.Configuration["OAuth:Domain"];
var audience = builder.Configuration["OAuth:Audience"];
var apiAppName = builder.Configuration["OAuth:AppName"];
var apiClientId = builder.Configuration["OAuth:Api:ClientId"];
var apiClientSecret = builder.Configuration["OAuth:Api:ClientSecret"];
var swaggerClientId = builder.Configuration["OAuth:Swagger:ClientId"];
var swaggerClientSecret = builder.Configuration["OAuth:Swagger:ClientSecret"];{% endif %}
{% if cookiecutter.include_azure_key_vault %}
//Key Vault Resource
var keyVault = builder.AddKeyVaultResource();
{% endif %}
{% if cookiecutter.include_azure_storage %}
//Storage Resource
var storageResource = builder.AddStorageResource();
{% endif %}
{% if cookiecutter.include_azure_application_insights %}
// Application Insights Resource
var insights = builder.AddAppInsightsResource();
{% endif %}
{% if cookiecutter.database == "PostgreSql" %}
//Database Resource
var projectdb = builder.AddPostgreSQLResource();
{% elif cookiecutter.database == "MongoDb" %}
var projectdb = builder.AddMongoDBResource();
{% endif %}

var apiServiceBuilder = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Api>("{{cookiecutter.assembly_name|lower }}-api")
    .WaitFor(projectdb)
    .WithReference(projectdb)
    .WithExternalHttpEndpoints()
    {% if cookiecutter.include_azure_key_vault %}
    .WaitFor(keyVault)
    .WithReference(keyVault)
    {% endif %}
{% if cookiecutter.include_azure_storage %}
    .WaitFor(storageResource.Blobs)
    .WithReference(storageResource.Blobs)
    {% endif %}
{% if cookiecutter.include_oauth %}
    .WithEnvironment("OAuth_Domain", issuer)
    .WithEnvironment("OAuth_Audience", audience)
    .WithEnvironment("OAuth_Api_AppName", apiAppName)
    .WithEnvironment("OAuth_Api_ClientId", apiClientId)
    .WithEnvironment("OAuth_Api_ClientSecret", apiClientSecret)
    .WithEnvironment("OAuth_Swagger_Id", swaggerClientId)
    .WithEnvironment("OAuth_Swagger_Secret", swaggerClientSecret)
    {% endif %}
    .WithHttpHealthCheck("/alive")
    .WithHttpHealthCheck("/health");

var migrationsBuilder = builder.AddProject<Projects.{{ cookiecutter.assembly_name }}_Migrations>("{{cookiecutter.assembly_name|lower }}-migrations")
    .WaitFor(projectdb)
    .WithReference(projectdb);
{% if cookiecutter.include_azure_application_insights %}
apiServiceBuilder = apiServiceBuilder.WaitFor(insights);
apiServiceBuilder = apiServiceBuilder.WithReference(insights);
migrationsBuilder = migrationsBuilder.WithReference(insights);
{% endif %}
builder.Build().Run();