
public class SampleService : ISampleService
{
    protected IMongoCollection<Sample> _sampleService;
    private readonly ILogger _logger;

     public SampleService( IMongoDbService context, ILogger<Sample> logger )
    {
        _sampleService = context.GetCollection<Sample>( "Sample" );
        _logger = logger;
    }

    public async Task CreateSampleAsync( Sample sample )
    {
        try
        {
            var existingSample = await _sampleService.AsQueryable().FirstOrDefaultAsync( x => x.Id == sample.Id );

            if ( existingSample == null )
            {
                await _sampleService.InsertOneAsync( sample );
            }
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
            var filter = Builders<Sample>.Filter.Eq( "Id", sampleId );
            {% endif %}
            var update = Builders<Sample>.Update.Set( x => x.Name, name ).Set( x => x.Description, description );

            await _sampleService.UpdateOneAsync( filter, update );

        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( UpdateSampleAsync ), "Error updating Sample.", ex );
        }
    }

    public async Task<SampleDefinition> GetSampleAsync( string sampleId )
    {
        try
        {
            var filter = Builders<Sample>.Filter.Eq( "Id", sampleId );
            var sample = await (await _sampleService.FindAsync( filter )).FirstOrDefaultAsync();
            if ( sample != null )
            {
                return new SampleDefinition(
                    sample.Id,
                    sample.Name,
                    sample.Description
              );
            }

            return new SampleDefinition( null, null, null );
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( GetSampleAsync ), "Error getting sample.", ex );
        }
    }

}