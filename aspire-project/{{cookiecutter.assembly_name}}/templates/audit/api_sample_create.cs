    private async Task<SampleDefinition> InsertSampleAsync(IPipelineContext context, Sample sample)
    {
        using (AuditScope.Create("Sample:Create", () => sample))
        {
            sample.Id = await _sampleService.CreateSampleAsync(sample);

            var sampleDefinition = new SampleDefinition
            (
                {% if cookiecutter.database == "PostgreSql" %}
                sample.Id,
                {% else %}
                sample.Id.ToString(),
                {% endif %} 
                sample.Name,
                sample.Description
            );
            return sampleDefinition;
        }
    }