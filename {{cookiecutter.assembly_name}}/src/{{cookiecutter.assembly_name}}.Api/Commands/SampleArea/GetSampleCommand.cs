using FluentValidation.Results;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

{%- if cookiecutter.database == "Postgresql" -%}

public interface IGetSampleCommand : ICommandFunction<int, SampleDefinition>;

public class GetSampleCommand : ServiceCommandFunction<int, SampleDefinition>, IGetSampleCommand
{
    private readonly ISampleService _sampleService;

    public GetSampleCommand(
        ISampleService sampleService,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<GetSampleCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
    }

    protected override FunctionAsync<int, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<int>()
            .WithLogging()
            .PipeAsync( GetSampleAsync )
            .Build();
    }
    private async Task<SampleDefinition> GetSampleAsync( IPipelineContext context, int sampleId )
    {
        var sample = await _sampleService.GetSampleAsync( sampleId );

        if ( sample != null )
            return sample;

        context.AddValidationResult( new ValidationFailure( nameof( sample ), "Sample does not exist" ) );
        context.CancelAfter();
        return null;
    }
}
   {%- elif cookiecutter.database == "Mongo" -%}

public interface IGetSampleCommand : ICommandFunction<string, SampleDefinition>;

public class GetSampleCommand : ServiceCommandFunction<string, SampleDefinition>, IGetSampleCommand
{
    private readonly ISampleService _sampleService;

    public GetSampleCommand(
        ISampleService sampleService,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<GetSampleCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
    }

    protected override FunctionAsync<string, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<string>()
            .WithLogging()
            .PipeAsync( GetSampleAsync )
            .Build();
    }
    private async Task<SampleDefinition> GetSampleAsync( IPipelineContext context, string sampleId )
    {
        var sample = await _sampleService.GetSampleAsync( sampleId );

        if ( sample != null )
            return sample;

        context.AddValidationResult( new ValidationFailure( nameof( sample ), "Sample does not exist" ) );
        context.CancelAfter();
        return null;
    }
}
   {% endif %}
