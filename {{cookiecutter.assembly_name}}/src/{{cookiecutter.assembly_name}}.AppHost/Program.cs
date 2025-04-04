using {{cookiecutter.assembly_name}}.AppHost;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

{% if cookiecutter.include_azure == "yes" %}
// Conditionally update the app model with secrets.
var secrets = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureKeyVault( "secrets" )
        : builder.AddConnectionString( "secrets" );

// Automatically provision an Application Insights resource
var appInsights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights( "appInsights" )
    : builder.AddConnectionString( "appInsights", "APPLICATIONINSIGHTS_CONNECTION_STRING" );

//Azure Storage
var storage = builder.AddAzureStorage( "storage" );

if ( builder.Environment.IsDevelopment() )
{
    storage.RunAsEmulator( az =>
    {
        az.WithDataBindMount();
    } );
}
{% endif %}

{% if cookiecutter.database == "PostgreSql" %}
var dbPassword = builder.AddParameter("DbPassword", "postgres", true);
var dbUser = builder.AddParameter("DbUser", "postgres", true);

var dbServer = builder.AddPostgres("postgres", userName: dbUser, password: dbPassword)
    .PublishAsConnectionString()
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("8.14"));

{% elif cookiecutter.database == "MongoDb" %}

var dbUsername = builder.AddParameter( "DbUser", "mongodb", true );
var dbPassword = builder.AddParameter( "DbPassword","mongodb", secret: true );

var dbServer = builder.AddMongoDB( "mongo", userName: dbUsername, password: dbPassword )
                .WithMongoExpress()
                .PublishAsConnectionString()
                .WithLifetime( ContainerLifetime.Persistent )
                .WithDataVolume();
{% endif %}

var projectdb = dbServer.AddDatabase("{{cookiecutter.database_name}}");

var apiService = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Api>("{{cookiecutter.assembly_name|lower }}-api")
    .WithReference(projectdb)
    .WithExternalHttpEndpoints()
    {% if cookiecutter.include_azure == "yes" %}
        .WithReference( secrets )
        .WithReference( appInsights )
    {% endif %}
    .WithSwaggerUI();

builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Migrations>("{{cookiecutter.assembly_name|lower }}-migrations")
    .WaitFor(projectdb)
     {% if cookiecutter.include_azure == "yes" %}
     .WithReference( appInsights )
     {% endif %}
    .WithReference( projectdb );  

builder.Build().Run();