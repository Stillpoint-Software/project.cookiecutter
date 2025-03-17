#nullable disable

using Hyperbee.Migrations;
using Hyperbee.Migrations.Providers.{{cookiecutter.database}};

namespace {{cookiecutter.assembly_name}}.Migrations.Migrations;


[Migration(1000)]
public class Initial({{cookiecutter.database}}ResourceRunner<Initial> resourceRunner) : Migration
{
    public override async Task UpAsync(CancellationToken cancellationToken = default)
    {
        // run a `resource` migration to create initial state.
        await resourceRunner.AllSqlFromAsync(cancellationToken);
    }
}

