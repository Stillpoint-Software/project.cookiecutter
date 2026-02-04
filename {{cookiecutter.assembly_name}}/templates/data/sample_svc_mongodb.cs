public class SampleService : ISampleService
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger _logger;

    public SampleService(DatabaseContext dbContext, ILogger<Sample> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SampleDefinition> GetSampleAsync(string sampleId)
    {
        try
        {
            if (!ObjectId.TryParse(sampleId, out var existingId))
                return null;

            var sample = await _dbContext.Samples
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingId);

            return sample is null
               ? null
               : new SampleDefinition(sample.Id.ToString(), sample.Name, sample.Description);
        }
        catch (Exception ex)
        {
            throw new ServiceException(nameof(GetSampleAsync), "Error getting sample.", ex);
        }
    }


    public async Task<string> CreateSampleAsync(Sample sample)
    {
        try
        {
            await _dbContext.Samples.AddAsync(sample);
            await _dbContext.SaveChangesAsync();

            return sample.Id.ToString();
        }
        catch (Exception ex)
        {
            throw new ServiceException(nameof(CreateSampleAsync), "Error saving sample.", ex);
        }
    }

    {%- if cookiecutter.include_audit %}
public async Task<SampleDefinition> UpdateSampleAsync(Sample existing, string sampleId, string name, string description)
{
    try
    {
        if (existing is null)
        {
            throw new ServiceException(nameof(UpdateSampleAsync), "Sample not found.");
        }

        var update = Builders<Sample>.Update.Set(x => x.Name, name).Set(x => x.Description, description);

        _dbContext.Entry(existing).CurrentValues.SetValues(new
        {
            Name = name,
            Description = description
        });

        await _dbContext.SaveChangesAsync();

        return new SampleDefinition
        (
            existing.Id.ToString(),
            existing.Name ?? string.Empty,
            existing.Description ?? string.Empty
        );
    }
    catch (Exception ex)
    {
        throw new ServiceException(nameof(UpdateSampleAsync), "Error updating Sample.", ex);
    }
}
{%- else %}
public async Task<SampleDefinition> UpdateSampleAsync(string sampleId, string name, string description)
{
    try
    {
        if (!ObjectId.TryParse(sampleId, out var oid))
            throw new ServiceException(nameof(UpdateSampleAsync), "Invalid sample id.");

        var existingSample = await _dbContext.Samples.FirstOrDefaultAsync(x => x.Id == oid);

        if (existingSample is null)
            throw new ServiceException(nameof(UpdateSampleAsync), "Sample not found.");

        existingSample.Name = name;
        existingSample.Description = description;

        await _dbContext.SaveChangesAsync();

        return new SampleDefinition(
            existingSample.Id.ToString(),
            existingSample.Name ?? string.Empty,
            existingSample.Description ?? string.Empty
        );
    }
    catch (Exception ex)
    {
        throw new ServiceException(nameof(UpdateSampleAsync), "Error updating Sample.", ex);
    }

}
{%- endif %}
}
