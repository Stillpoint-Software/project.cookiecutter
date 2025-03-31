
public class SampleService : ISampleService
{
    private readonly SampleContext _sampleContext;
    private readonly ILogger _logger;

    public SampleService( SampleContext sampleContext, ILogger<Sample> logger )
    {
        _sampleContext = sampleContext;
        _logger = logger;
    }

    public async Task<SampleDefinition> GetSampleAsync( int sampleId )
    {
        try
        {
            return await _sampleContext.Sample
                  .Where( x => x.Id == sampleId )
                  .Select( x => new SampleDefinition(
                      x.Id,
                      x.Name ?? string.Empty,
                      x.Description ?? string.Empty
                  ) )
                  .FirstOrDefaultAsync() ?? throw new ServiceException( nameof( GetSampleAsync ), "Sample not found." );
        }
        catch (Exception ex)
        {
            throw new ServiceException( nameof( GetSampleAsync ), "Error getting sample.", ex );
        }
    }

    {% if cookiecutter.include_audit == "yes" %}
    {% include '/templates/audit/data_sample_svc_postgresql.cs' %}
    {% else %}
    public async Task<int> CreateSampleAsync( Sample sample )
    {
        try
        {
            _sampleContext.Sample.Add( sample );
            await _sampleContext.SaveChangesAsync();
            return sample.Id;
        }
        catch (Exception ex)
        {
            throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
        }
    }

   public async Task UpdateSampleAsync( int sampleId, string name, string description )
    {
        try
        {
            await _sampleContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ServiceException( nameof( UpdateSampleAsync ), "Error updating Sample.", ex );
        }
    }
       {% endif %}
}
