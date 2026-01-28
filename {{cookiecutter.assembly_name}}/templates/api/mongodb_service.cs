builder.AddMongoDBClient("{{cookiecutter.database_name | lower}}");
builder.Services.AddScoped<DatabaseContext>(svc =>
{
    var scope = svc.CreateScope();
    return DatabaseContext.Create(scope.ServiceProvider.GetRequiredService<IMongoDatabase>());
});