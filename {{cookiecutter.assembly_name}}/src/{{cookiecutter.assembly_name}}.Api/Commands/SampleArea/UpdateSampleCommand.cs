{% if cookiecutter.include_audit =='yes'%}
using Audit.Core;
{% endif %}
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

public record UpdateSample( int sampleId, string Name, string Description );

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
            {% if cookiecutter.include_audit =='yes'%}
            var sampleOriginal = await _patientService.GetSampleAsync(update.sampleId);

        if (sample == null)
            return null;

        var updatedSample = await AuditScope.CreateAsync( c => c
        .EventType( "Sample:Update" )
           .AuditEvent( new ListAuditEvent( sampleOriginal ) )
           .IsCreateAndSave() );

        return updatedSample;
        {% else %}
        await _sampleService.UpdateSampleAsync( update.sampleId, update.Name, update.Description );

        return new SampleDefinition(
            update.sampleId,
            update.Name,
            update.Description
        );
        {% endif %}
    }
}
