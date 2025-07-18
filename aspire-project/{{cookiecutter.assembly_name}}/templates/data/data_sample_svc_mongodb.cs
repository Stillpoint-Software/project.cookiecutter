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
            var sample = await _dbContext.Samples.Where(x => x.Id.ToString() == sampleId)
              .FirstOrDefaultAsync();

            if (sample != null)
            {
                return new SampleDefinition(
                    sample.Id.ToString(),
                    sample.Name,
                    sample.Description
              );
            }

            return new SampleDefinition(sample.Id.ToString(), sample.Name, sample.Description);
        }
        catch (Exception ex)
        {
            throw new ServiceException(nameof(GetSampleAsync), "Error getting sample.", ex);
        }
    }


    public async Task<ObjectId> CreateSampleAsync(Sample sample)
    {
        try
        {
            var existingSample = await _dbContext.Samples.Where(x => x.Id == sample.Id)
             .FirstOrDefaultAsync();

            if (existingSample == null)
            {
                await _dbContext.Samples.AddAsync(sample);
                await _dbContext.SaveChangesAsync();
                return sample.Id;
            }
            return existingSample.Id;
        }
        catch (Exception ex)
        {
            throw new ServiceException(nameof(CreateSampleAsync), "Error saving sample.", ex);
        }
    }

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
}