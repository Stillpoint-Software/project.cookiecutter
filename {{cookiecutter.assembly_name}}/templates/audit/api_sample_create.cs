 private async Task<SampleDefinition> InsertSampleAsync( IPipelineContext context, Data.Abstractions.Entity.Sample sample )
    {
        using (AuditScope.Create( "Sample:Create", () => sample ))
        {
            sample.Id = await _sampleService.CreateSampleAsync( sample );

            var sampleDefinition = new SampleDefinition
            (
                 sample.Id,
                 sample.Name,
                 sample.Description
            );
            return sampleDefinition;
        }
    }