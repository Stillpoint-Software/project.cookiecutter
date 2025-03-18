    public async Task<string> CreateSampleAsync( Sample sample )
        {
            try
            {
                var existingSample = await _sampleService.AsQueryable().FirstOrDefaultAsync( x => x.Id == sample.Id );

                if ( existingSample == null )
                {
                    await _sampleService.InsertOneAsync( sample );
                }
                return sample.Id;
            }
            catch ( Exception ex )
            {
                throw new ServiceException( nameof( CreateSampleAsync ), "Error saving sample.", ex );
            }
        }

        public async Task<SampleDefinition> UpdateSampleAsync( FilterDefinition<Sample> existing, string sampleId, string name, string description )
    {

        try
        {
            if ( existing is null )
            {
                throw new ServiceException( nameof( UpdateSampleAsync ), "Sample not found." );
            }

            var update = Builders<Sample>.Update.Set( x => x.Name, name ).Set( x => x.Description, description );

            await _sampleService.UpdateOneAsync( existing, update );
            return new SampleDefinition( sampleId, name, description );
        }
        catch ( Exception ex )
        {
            throw new ServiceException( nameof( UpdateSampleAsync ), "Error updating Sample.", ex );
        }
    }

