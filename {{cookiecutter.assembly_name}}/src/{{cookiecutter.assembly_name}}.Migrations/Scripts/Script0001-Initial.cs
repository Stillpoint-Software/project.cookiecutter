using Hyperbee.Migrations;
{% if cookiecutter.database == "Postgresql" %}
using Hyperbee.Migrations.Providers.Postgres.Resources;
{% elif cookiecutter.database == "MongoDb" %}
using Hyperbee.Migrations.Providers.MongoDB.Resources;
{% endif %}


namespace {{cookiecutter.assembly_name}}.Migrations.Scripts;
[Migration( 1000 )]
{% if cookiecutter.database == "Postgresql" %}
public class Initial( PostgresResourceRunner<Initial> resourceRunner ) : Migration
{
     public override async Task UpAsync( CancellationToken cancellationToken = default )
    {
        // run a `resource` migration to create initial state.
        await resourceRunner.AllSqlFromAsync( cancellationToken );
    }
{% elif cookiecutter.database == "MongoDb" %}
public class Initial( MongoDBResourceRunner<Initial> resourceRunner ) : Migration
    {
    public override async Task UpAsync( CancellationToken cancellationToken = default )
    {
        // run a `resource` migration to create initial state.
        await resourceRunner.DocumentsFromAsync( [
            "administration/users/user.json"
        ], cancellationToken );
    }
   {% endif %}
}
