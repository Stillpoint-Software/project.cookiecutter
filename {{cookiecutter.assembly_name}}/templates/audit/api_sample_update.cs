

public record UpdateSample( int sampleId, string Name, string Description );

public interface IUpdateSampleCommand : ICommandFunction<UpdateSample, SampleDefinition>;

public class UpdateSampleCommand : ServiceCommandFunction<UpdateSample, SampleDefinition>, IUpdateSampleCommand
{
    private readonly ISampleService _sampleService;
     private readonly SampleContext _sampleContext;

    public UpdateSampleCommand(
        ISampleService sampleService,
        SampleContext sampleContext,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<UpdateSampleCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
        _sampleContext = sampleContext;
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
       
  
    var original = await _sampleContext.Sample.FindAsync( update.sampleId ) ?? throw new Exception( "Sample not found" );

      //var original = await _sampleService.GetSampleAsync( update.Id );  // This is an issue with updating in the service as it is not being tracked by the context

      using (AuditScope.Create( "Patients:Update", () => original ))
      {
          var updatedSample = await _sampleService.UpdateSampleAsync( original, update.sampleId, update.Name, update.Description );

          return updatedSample;
      }
    }
}
  
  
  