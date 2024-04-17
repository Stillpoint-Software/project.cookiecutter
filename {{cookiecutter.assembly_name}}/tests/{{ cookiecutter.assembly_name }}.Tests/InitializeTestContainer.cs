using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using Testcontainers.PostgreSql;

namespace {{cookiecutter.assembly_name}}.Tests;

[TestClass]
public class InitializeTestContainer
{
    public static IDbConnectionProvider ConnectionProvider { get; set; }

    [AssemblyInitialize]
    public static async Task Initialize( TestContext context )
    {
        var cancellationToken = context.CancellationTokenSource.Token;

        var network = new NetworkBuilder()
            .WithName( Guid.NewGuid().ToString( "D" ) )
            .WithCleanUp( true )
            .Build();

        await network.CreateAsync( cancellationToken )
            .ConfigureAwait( false );

        var postgresContainer = new PostgreSqlBuilder()
            .WithNetwork( network )
            .WithNetworkAliases( "db" )
            .WithDatabase( "{{cookiecutter.database}}" )
            .WithUsername( "test" )
            .WithPassword( "test" )
            .WithPortBinding( 6543, 5432 )
            .WithCleanUp( true )
            .WithWaitStrategy( Wait.ForUnixContainer().UntilPortIsAvailable( 5432 ) )
            .Build();

        var location = CommonDirectoryPath.GetSolutionDirectory();
        var image = new ImageFromDockerfileBuilder()
            .WithDeleteIfExists( true )
            .WithCleanUp( true )
            .WithName( "db-migrations" )
            .WithDockerfile( "src/{{cookiecutter.assembly_name}}.Migrations/Dockerfile" )
            .WithDockerfileDirectory( location.DirectoryPath )
            .Build();

        await image.CreateAsync( cancellationToken )
            .ConfigureAwait( false );

        await postgresContainer.StartAsync( cancellationToken )
            .ConfigureAwait( false );

        var migrationContainer = new ContainerBuilder()
            .WithCleanUp( true )
            .WithNetwork( network )
            .WithImage( image )
            .WithEnvironment( "{{cookiecutter.database}}__ConnectionString", "Server=db;Port=5432;Database=postgres;User Id=test;Password=test;" )
            .WithWaitStrategy( Wait.ForUnixContainer().AddCustomWaitStrategy( new WaitUntilExited() ) )
            .Build();

        await migrationContainer.StartAsync( cancellationToken )
            .ConfigureAwait( false );

        ConnectionProvider = new NpgsqlConnectionProvider( postgresContainer.GetConnectionString() + ";Include Error Detail=true" );
    }

    public class WaitUntilExited : IWaitUntil
    {
        public async Task<bool> UntilAsync( IContainer container )
        {
            await Task.CompletedTask;
            return container.State == TestcontainersStates.Exited;
        }
    }
}
