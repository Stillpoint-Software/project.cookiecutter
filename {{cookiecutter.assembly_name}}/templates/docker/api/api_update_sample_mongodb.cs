
{% if cookiecutter.include_audit =='yes'%}
{% include 'templates/audit/api_sample_update_mongodb.cs' %}
{% else %}

public record UpdateSample( string sampleId, string Name, string Description );

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
        var filter = Builders<Sample>.Filter.Eq( "Id", update.sampleId );
        await _sampleService.UpdateSampleAsync( filter, update.sampleId, update.Name, update.Description );
        return new SampleDefinition(
            update.sampleId,
            update.Name,
            update.Description
        );
    }
}
{% endif %}