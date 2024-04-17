using {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

public record UpdateSample( int Id, string Name, string Description );

public interface IUpdateSampleCommand : ICommandFunction<UpdateSample, SampleDefinition>;

public class UpdateSampleCommand : ServiceCommandFunction<UpdateSample, SampleDefinition>, IUpdateSampleCommand
{
    private readonly ISampleService _sampleService;

    public UpdateSampleCommand(
        ISampleService sampleService,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<UpdateSampleCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
    }

    protected override FunctionAsync<UpdateSample, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<UpdateSample>()
            .WithLogging()
            .CancelOnFailure( Validate<UpdateSample> )
            .PipeAsync( UpdateSampleAsync )
            .Build();
    }

    private async Task<SampleDefinition> UpdateSampleAsync( IPipelineContext context, UpdateSample update )
    {
        var updateSample = await _sampleService.UpdateSampleAsync( update.Id, update.Name, update.Description );
        if ( updateSample == null )
            return null;

        return new SampleDefinition(
            updateSample.SampleId,
            updateSample.Name,
            updateSample.Description
        );
    }
}

