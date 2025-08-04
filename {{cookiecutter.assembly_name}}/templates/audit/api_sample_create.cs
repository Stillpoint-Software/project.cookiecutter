    private async Task<SampleDefinition> InsertSampleAsync(IPipelineContext context, Sample sample)
    {
        using (AuditScope.Create("Sample:Create", () => sample))
        {
            {% if cookiecutter.database =="MongoDb" %}
            sample.Id = ObjectId.Parse(await _sampleService.CreateSampleAsync(sample));
            {% else %}
            var sampleId = await _sampleService.CreateSampleAsync(sample);
            {% endif %}

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