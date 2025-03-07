using {{cookiecutter.assembly_name}}.AppHost;

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
var storage = builder.AddAzureStorage( "storage" )
                     .RunAsEmulator();

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
                .PublishAsConnectionString()
                .WithLifetime( ContainerLifetime.Persistent )
                .WithDataVolume();
{% endif %}

var projectdb = dbServer.AddDatabase("projectdb");

var apiService = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Api>({{cookiecutter.assembly_name}}"-api")
    .WithReference(projectdb)
    {% if cookiecutter.include_azure == "yes" %}
        .WithReference( keyVault )
        .WithReference( appInsights )
        .WithReference( storage )
    {% endif %}
    .WithSwaggerUI();

builder.AddProject<Projects.{{cookiecutter.assembly_name}}_Migrations>({{cookiecutter.assembly_name}}"-migrations")
    .WaitFor(projectdb)
    .WithReference(projectdb)
     {% if cookiecutter.include_azure == "yes" %}
        .WithReference( appInsights )
    {% endif %}
    .WithHttpHealthCheck( "/health" );

builder.Build().Run();