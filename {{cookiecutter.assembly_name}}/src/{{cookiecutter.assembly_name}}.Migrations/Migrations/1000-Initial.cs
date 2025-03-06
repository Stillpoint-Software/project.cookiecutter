#nullable disable

using Hyperbee.Migrations;
using Hyperbee.Migrations.Providers.Postgres.Resources;

namespace {{cookiecutter.assembly_name}}.MigrationService.Migrations;


[Migration(1000)]
public class Initial(PostgresResourceRunner<Initial> resourceRunner) : Migration
{
    public override async Task UpAsync(CancellationToken cancellationToken = default)
    {
        // run a `resource` migration to create initial state.
        await resourceRunner.AllSqlFromAsync(cancellationToken);
    }
}