using {{cookiecutter.assembly_name}}.Api.Identity;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

{%- if cookiecutter.database == "Mongo" -%}
using MongoDB.Bson;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

public record CreateSample( string Name, string Description );

public interface ICreateSampleCommand : ICommandFunction<CreateSample, SampleDefinition>;

public class CreateSampleCommand : ServiceCommandFunction<CreateSample, SampleDefinition>, ICreateSampleCommand
{
    private readonly ISampleService _sampleService;
    private readonly string _user;

    public CreateSampleCommand(
        ISampleService sampleService,
        IPrincipalProvider principalProvider,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<CreateSampleCommand> logger ) :
        base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
        _user = principalProvider.GetEmail();
    }

    protected override FunctionAsync<CreateSample, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<CreateSample>()
            .WithLogging()
            .PipeAsync( CreateSampleAsync )
            .CancelOnFailure( Validate<Sample> )
            .PipeAsync( InsertSampleAsync )
            .Build();
    }

    {%- if cookiecutter.database == "Postgresql" -%}
 
  private async Task<Sample> CreateSampleAsync( IPipelineContext context, CreateSample sample )
    {

        return await Task.FromResult( new Sample
        {
            Name = sample.Name,
            Description = sample.Description,
        } );
    }

    private async Task<SampleDefinition> InsertSampleAsync( IPipelineContext context, Sample sample )
    {
        sample.Id = await _sampleService.CreateSampleAsync( sample );

        return new SampleDefinition(
            sample.Id,
            sample.Name,
            sample.Description
        );
    }

   {%- elif cookiecutter.database == "Mongo" -%}
    private async Task<Sample> CreateSampleAsync( IPipelineContext context, CreateSample sample )
    {
        return await Task.FromResult( new Sample
        {
            Name = sample.Name,
            Description = sample.Description,
            CreatedBy = _user
        } );
    }

    private async Task<SampleDefinition> InsertSampleAsync( IPipelineContext context, Sample sample )
    {
        await _sampleService.CreateSampleAsync( sample );

        return new SampleDefinition(
            sample.Id,
            sample.Name,
            sample.Description
        );
    }
   {% endif %}
}
