{% if cookiecutter.include_audit =='yes'%}
using Audit.Core;
{% endif %}
using FluentValidation.Results;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

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
        {% if cookiecutter.include_audit =='yes'%}
        var sample = await _sampleService.GetSampleAsync(sampleId);

        if (sample != null)
            return null;

        var test = await AuditScope.CreateAsync( c => c
        .EventType( "Sample:Get" )
           .AuditEvent( new ListAuditEvent( sample ) )
           .IsCreateAndSave() );

        return sample;
        {% else %}

        var sample = await _sampleService.GetSampleAsync( sampleId );

        if (sample == null)
            return null;

        context.AddValidationResult( new ValidationFailure( nameof( sample ), "Sample does not exist" ) );
        context.CancelAfter();
        return null;
        {% endif %}
    }
}