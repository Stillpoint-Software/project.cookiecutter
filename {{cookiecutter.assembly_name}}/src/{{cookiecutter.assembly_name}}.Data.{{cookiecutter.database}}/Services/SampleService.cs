using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;
{% if cookiecutter.database == "PostgreSql" %}
using Microsoft.EntityFrameworkCore;
{% elif cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using MongoDB.Driver.Linq;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;

{% if cookiecutter.database == "PostgreSql" %}
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
            _sampleContext.Sample.Add( sample );
            await _sampleContext.SaveChangesAsync();
            return sample.Id;
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
        }
    }

    public async Task UpdateSampleAsync( int sampleId, string name, string description )
    {
        try
        {
            var existing = await _sampleContext.Sample.FirstOrDefaultAsync( x => x.Id == sampleId );
            if ( existing == null )
                return ;

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
{% elif cookiecutter.database == "MongoDb" %}

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

    public async Task UpdateSampleAsync( string sampleId, string name, string description )
    {
        try
        {
            var filter = Builders<Sample>.Filter.Eq( "Id", sampleId );
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
{% endif %}
