using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Data.Postgresql.Services;


public class SampleService( SampleContext sampleContext, ILogger<SampleService> logger ) : ISampleService
{
    private readonly SampleContext _sampleContext = sampleContext;
    private readonly ILogger _logger = logger;

    public async Task<int> CreateSampleAsync( Sample sample )
    {
        try
        {
            _sampleContext.Sample.Add( sample );
            await _sampleContext.SaveChangesAsync();
            return sample.SampleId;
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
        }
    }

    public IAsyncEnumerable<SampleDefinition> GetAllSamplesAsync()
    {
        try
        {
            return _sampleContext.Sample
                .Select( x => new SampleDefinition(
                    x.SampleId,
                    x.Name,
                    x.Description
                ) )
                .AsAsyncEnumerable();
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( GetAllSamplesAsync ), "Error getting all sample.", ex );
        }
    }

    public async Task<Sample> UpdateSampleAsync( int sampleId, string name, string description )
    {
        try
        {
            var existing = await _sampleContext.Sample.FirstOrDefaultAsync( x => x.SampleId == sampleId );
            if ( existing == null )
                return null;

            _sampleContext.Entry( existing ).CurrentValues.SetValues( new
            {
                Name = name,
                Description = description
            } );

            await _sampleContext.SaveChangesAsync();
            return existing;
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
                .Where( x => x.SampleId == sampleId )
                .Select( x => new SampleDefinition(
                    x.SampleId,
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
