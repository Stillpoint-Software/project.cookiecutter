
public class SampleService : ISampleService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger _logger;

    public SampleService(DatabaseContext databaseContext, ILogger<Sample> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<SampleDefinition> GetSampleAsync(int sampleId)
    {
        try
        {
            return await _databaseContext.Samples
                  .Where(x => x.Id == sampleId)
                  .Select(x => new SampleDefinition(
                      x.Id,
                      x.Name ?? string.Empty,
                      x.Description ?? string.Empty
                  ))
                  .FirstOrDefaultAsync() ?? throw new ServiceException(nameof(GetSampleAsync), "Sample not found.");
        }
        catch (Exception ex)
        {
            throw new ServiceException(nameof(GetSampleAsync), "Error getting sample.", ex);
        }
    }

    {% if cookiecutter.include_audit == "yes" %}
{% include 'templates/audit/data_sample_svc_postgresql.cs' %}
{% else %}
public async Task<int> CreateSampleAsync(Sample sample)
{
    try
    {
        _databaseContext.Samples.Add(sample);
        await _databaseContext.SaveChangesAsync();
        return sample.Id;
    }
    catch (Exception ex)
    {
        throw new ServiceException(nameof(CreateSampleAsync), "Error saving sample.", ex);
    }
}

public async Task UpdateSampleAsync(int sampleId, string name, string description)
{
    try
    {
        await _databaseContext.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        throw new ServiceException(nameof(UpdateSampleAsync), "Error updating Sample.", ex);
    }
}
{% endif %}
}
