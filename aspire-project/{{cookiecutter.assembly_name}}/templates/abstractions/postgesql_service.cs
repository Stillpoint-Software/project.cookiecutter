Task<int> CreateSampleAsync(Sample sample);
Task<SampleDefinition> GetSampleAsync(int sampleId);
{% if cookiecutter.include_audit == "yes" %}
Task<SampleDefinition> UpdateSampleAsync(Sample existing, int sampleId, string name, string description);
{% else %}
Task UpdateSampleAsync(int sampleId, string name, string description);
{% endif %}