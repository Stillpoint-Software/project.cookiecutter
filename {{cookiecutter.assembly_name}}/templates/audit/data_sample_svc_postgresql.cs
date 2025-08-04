
    public async Task<int> CreateSampleAsync( Sample sample )
    {
        try
        {
            _databaseContext.Samples.Add( sample );
            await _databaseContext.SaveChangesAsync();
            return sample.Id;
        }
        catch (Exception ex)
        {
            throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
        }
    }

   public async Task<SampleDefinition> UpdateSampleAsync( Sample existing, int sampleId, string name, string description )
    {
        try
        {
            if (existing == null)
                throw new ServiceException( nameof( UpdateSampleAsync ), "Sample cannot be null." );

            _databaseContext.Entry( existing ).CurrentValues.SetValues( new
            {
                Name = name,
                Description = description
            } );

            await _databaseContext.SaveChangesAsync();

            return new SampleDefinition(
                existing.Id,
                existing.Name ?? string.Empty,
                existing.Description ?? string.Empty
            );
        }
        catch (Exception ex)
        {
            throw new ServiceException( nameof( UpdateSampleAsync ), "Error updating Sample.", ex );
        }
    }