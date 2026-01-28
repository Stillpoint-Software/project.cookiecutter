public record UpdateSample( string sampleId, string Name, string Description );

public interface IUpdateSampleCommand : ICommandFunction<UpdateSample, SampleDefinition>;

public class UpdateSampleCommand : ServiceCommandFunction<UpdateSample, SampleDefinition>, IUpdateSampleCommand
{
    private readonly ISampleService _sampleService;
     private readonly DatabaseContext _databaseContext;

    public UpdateSampleCommand(
        ISampleService sampleService,
        DatabaseContext databaseContext,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<UpdateSampleCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
        _databaseContext = databaseContext;
    }

    protected override FunctionAsync<UpdateSample, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<UpdateSample>()
            .WithLogging()
            .PipeAsync( UpdateSampleAsync )
            .Build();
    }

    private async Task<SampleDefinition> UpdateSampleAsync(IPipelineContext context, UpdateSample update)
    {
        var original = await _databaseContext.Samples.FindAsync(ObjectId.Parse(update.sampleId)) ?? throw new ServiceException(nameof(UpdateSampleAsync), "Error updating Samples.");

        using (AuditScope.Create("Sample:Update", () => original))
        {
            var updatedSample = await _sampleService.UpdateSampleAsync(original, update.sampleId, update.Name, update.Description);

            return updatedSample;
        }
    }
}
  
  
  