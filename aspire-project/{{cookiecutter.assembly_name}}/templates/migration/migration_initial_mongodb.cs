using Hyperbee.Migrations;
using Hyperbee.Migrations.Providers.MongoDB.Resources;


namespace {{cookiecutter.assembly_name}}.Migrations.Migrations;

[Migration( 1000 )]
public class Initial( MongoDBResourceRunner<Initial> resourceRunner ) : Migration
{
    public override async Task UpAsync( CancellationToken cancellationToken = default )
    {
        // run a `resource` migration to create initial state.
        //Uncomment once the database is created.
        await resourceRunner.DocumentsFromAsync( ["samples/sample.json"], cancellationToken );
    }
}