using Microsoft.Extensions.Logging;
using NSubstitute;
using {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;
using {{cookiecutter.assembly_name}}.Api.Validators;
using {{cookiecutter.assembly_name}}.Core.Identity;
using {{cookiecutter.assembly_name}}.Core.Validators;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Tests.TestSupport;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Tests.Commands.SampleArea;

[TestClass]
public class CreateSampleCommandTests
{

   {% if cookiecutter.database == "MongoDb" %}
       {% include 'templates/tests/mongodb_setup.cs' %}
   {% endif %}
    // test support
    private static ICreateSampleCommand CreateCommand()
    {
        var logger = Substitute.For<ILogger<CreateSampleCommand>>();

        var validatorProvider = Substitute.For<IValidatorProvider>();
        validatorProvider.For<Sample>().Returns( new SampleValidation() );

        var prinicipalProvider = Substitute.For<IPrincipalProvider>();

        var pipelineContextFactory = PipelineContextFactoryFixture.Next( validatorProvider: validatorProvider );
        var sampleService = Substitute.For<ISampleService>();

        return new CreateSampleCommand( sampleService, prinicipalProvider, pipelineContextFactory, logger );
    }

    [TestMethod]
    public async Task Should_execute_command()
    {
        // arrange
        var command = CreateCommand();

        // act
        var result = await command.ExecuteAsync( new CreateSample( "Test Sample", "This is a test") );

        // assert
        Assert.IsTrue( result.Context.Success, result.ContextMessage() );
    }

    [TestMethod]
    public async Task Should_Create_Sample()
    {
        // arrange
        var command = CreateCommand();

        // act
        var result = await command.ExecuteAsync( new CreateSample( "Test Sample", "This is a test") );

        // assert
        Assert.IsTrue( result.Context.Success, result.ContextMessage() );
    }

    [TestMethod]
    public async Task Should_Fail_Create_Sample()
    {
        // arrange
        var command = CreateCommand();

        // act
        var result = await command.ExecuteAsync( new CreateSample( null, "This is a test" ) );

        // assert
        Assert.IsFalse( result.Context.Success, result.ContextMessage() );
    }
}
