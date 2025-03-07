
public class SampleService : ISampleService
{
    private readonly SampleContext _sampleContext;
    private readonly ILogger _logger;

    public SampleService( SampleContext sampleContext, ILogger<Sample> logger )
    {
        _sampleContext = sampleContext;
        _logger = logger;
    }

    public async Task<int> CreateSampleAsync( Sample sample )
    {
        try
        {
            await _sampleContext.Sample.AddAsync( sample );
            await _sampleContext.SaveChangesAsync();
            return sample.Id;
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
        }
    }


    {% if cookiecutter.include_audit == 'yes' %}
    public async Task UpdateSampleAsync( Sample existing, int sampleId, string name, string description )
    {% else %}
       public async Task UpdateSampleAsync( int sampleId, string name, string description )
    {% endif %}
    {
        try
        {
            {% if cookiecutter.include_audit == 'yes' %}
            if (existing is null)
            {
                throw new ServiceException( nameof( UpdateSampleAsync ), "Sample not found." );
            }
            {% else %}
            var existing = await _sampleContext.Sample.FirstOrDefaultAsync( x => x.Id == sampleId );
            if ( existing == null )
                return ;
            {% endif %}

            _sampleContext.Entry( existing ).CurrentValues.SetValues( new
            {
                Name = name,
                Description = description
            } );

            await _sampleContext.SaveChangesAsync();

        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( UpdateSampleAsync ), "Error updating Sample.", ex );
        }
    }

    public async Task<SampleDefinition> GetSampleAsync( int sampleId )
    {
        try
        {
            return await _sampleContext.Sample
                .Where( x => x.Id == sampleId )
                .Select( x => new SampleDefinition(
                    x.Id,
                    x.Name,
                    x.Description
                ) )
                .FirstOrDefaultAsync();
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( GetSampleAsync ), "Error getting sample.", ex );
        }
    }
}