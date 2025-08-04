
Task<string> CreateSampleAsync(Sample sample);
Task<SampleDefinition> GetSampleAsync(string sampleId);
{% if cookiecutter.include_audit == "yes" %}
Task<SampleDefinition> UpdateSampleAsync(Sample existing, string sampleId, string name, string description);
{% else %}
Task<SampleDefinition> UpdateSampleAsync(string sampleId, string name, string description);
{% endif %}
