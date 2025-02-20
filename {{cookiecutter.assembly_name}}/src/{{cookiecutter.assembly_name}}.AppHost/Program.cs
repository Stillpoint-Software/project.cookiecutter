using {{cookiecutter.assembly_name}}.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("DbPassword", "postgres", true);
var dbUser = builder.AddParameter("DbUser", "postgres", true);

var dbServer = builder.AddPostgres("postgres", userName: dbUser, password: dbPassword)
    .PublishAsConnectionString()
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("8.14"));

var projectdb = dbServer.AddDatabase("projectdb");

var apiService = builder.AddProject<Projects.{{cookiecutter.assembly_name}}_ApiService>("apiservice")
    .WithReference(projectdb)
    .WithSwaggerUI();

builder.AddProject<Projects.{{cookiecutter.assembly_name}}_MigrationService>({{cookiecutter.assembly_name}}"-migrationservice")
    .WaitFor(projectdb)
    .WithReference(projectdb);

builder.Build().Run();