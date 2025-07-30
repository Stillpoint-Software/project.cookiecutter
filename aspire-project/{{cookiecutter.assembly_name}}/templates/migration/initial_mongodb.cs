using Hyperbee.Migrations;
using Hyperbee.Migrations.Providers.MongoDB.Resources;


namespace {{cookiecutter.assembly_name}}.Migrations.Migrations;

[Migration( 1000 )]
public class Initial( MongoDBResourceRunner<Initial> resourceRunner ) : Migration
{
    public override async Task UpAsync( CancellationToken cancellationToken = default )
    {
        await resourceRunner.DocumentsFromAsync( ["{{cookiecutter.database_name}}/sample/sample.json"], cancellationToken );
    }
}