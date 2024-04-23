using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using Microsoft.EntityFrameworkCore;

namespace {{cookiecutter.assembly_name}}.Tests;

[TestClass]
public class MigrationRunner
{
       {%- if cookiecutter.database == "Postgresql" -%}
  
    [TestMethod]
    public async Task Should_Initialize_Tables()
    {
        var connectionProvider = InitializeTestContainer.ConnectionProvider;


        var sampleContext = new SampleContext(
            new DbContextOptionsBuilder<SampleContext>()
                .UseNpgsql(
                    connectionProvider.GetConnection().ConnectionString,
                    b => b.MigrationsAssembly( "TravelIntelligenceApi" ) )
                .Options, null );

        var samples = await sampleContext.Sample.ToListAsync();

        Assert.AreEqual( 0, samples.Count );
    }
    {% endif %}
}
