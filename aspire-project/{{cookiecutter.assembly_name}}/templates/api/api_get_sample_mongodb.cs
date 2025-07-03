
public interface IGetSampleCommand : ICommandFunction<string, SampleDefinition>;

public class GetSampleCommand : ServiceCommandFunction<string, SampleDefinition>, IGetSampleCommand
{
    private readonly ISampleService _sampleService;

    public GetSampleCommand(
        ISampleService sampleService,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<GetSampleCommand> logger)
        : base(pipelineContextFactory, logger)
    {
        _sampleService = sampleService;
    }

    protected override FunctionAsync<string, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<string>()
            .WithLogging()
            .PipeAsync(GetSampleAsync)
            .Build();
        }
    {% if cookiecutter.include_audit == 'yes' %}
    {% include "templates/audit/api_sample_get_mongodb.cs" %}
    {% else %}
    private async Task<SampleDefinition> GetSampleAsync(IPipelineContext context, string sampleId)
    {
        var sample = await _sampleService.GetSampleAsync(sampleId);

        context.AddValidationResult(new ValidationFailure(nameof(sample), "Sample does not exist"));
        context.CancelAfter();
        return null;
    }
    {% endif %}
}
