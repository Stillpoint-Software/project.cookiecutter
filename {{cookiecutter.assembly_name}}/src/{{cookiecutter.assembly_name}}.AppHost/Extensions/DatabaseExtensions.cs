using Aspire.Hosting.ApplicationModel;

namespace {{cookiecutter.assembly_name }}.AppHost.Extensions;

{%- if cookiecutter.database == "PostgreSql" %}
public static class PostgresDatabaseExtensions
{
    public static IResourceBuilder<PostgresDatabaseResource> AddPostgreSQLResource(
       this IDistributedApplicationBuilder builder)
    {
        var dbPassword = builder.AddParameter("DbPassword", "postgres", true);
        var dbUser = builder.AddParameter("DbUser", "postgres", true);


        var dbServer = builder.AddPostgres("postgres", userName: dbUser, password: dbPassword)
            .PublishAsConnectionString()
            .WithDataVolume("postgres-data")
            .WithContainerName("postgres-container")
            .WithPgAdmin(pgadmin =>
            {
                pgadmin
            .WithImageTag("latest")
            .WithImagePullPolicy(ImagePullPolicy.Always);
            });

        return dbServer.AddDatabase("{{ cookiecutter.database_name|lower }}");
    }

}
{% elif cookiecutter.database == "MongoDb" %}
public static class MongoDatabaseExtensions
{
    public static IResourceBuilder<MongoDBDatabaseResource> AddMongoDBResource(
       this IDistributedApplicationBuilder builder)
    {
        var dbUsername = builder.AddParameter("DbUser", "mongodb", true);
        var dbPassword = builder.AddParameter("DbPassword", "mongodb", secret: true);

        var dbServer = builder.AddMongoDB("mongo", userName: dbUsername, password: dbPassword)
                        .WithMongoExpress()
                        .WithImageTag("latest")
                        .PublishAsConnectionString()
                        .WithDataVolume("mongo-data")
                        .WithContainerName("mongo-container")
                        .WithLifetime(ContainerLifetime.Persistent);

        return dbServer.AddDatabase("{{ cookiecutter.database_name|lower }}");
    }
}
{%- endif %}
