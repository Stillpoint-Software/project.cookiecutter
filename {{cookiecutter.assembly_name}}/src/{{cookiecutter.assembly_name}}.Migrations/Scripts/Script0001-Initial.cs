using {{cookiecutter.assembly_name}}.Migrations.System;
using MongoDB.Driver;

namespace {{cookiecutter.assembly_name}}.Migrations.Scripts;

[Migration( 1000 )]
internal class InitialMigration : Migration
{
    private readonly IMongoDatabase _database;

    public InitialMigration( IMigrationDatabaseProvider provider )
    {
        _database = provider.GetDatabase();
    }

    public override async Task UpAsync()
    {
        await Task.CompletedTask;
    }
}
