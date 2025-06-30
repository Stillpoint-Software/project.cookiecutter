using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
public interface ISampleService
{
  
   {% if cookiecutter.database == "PostgreSql" %}
   Task<int> CreateSampleAsync( Sample sample );
   Task<SampleDefinition> GetSampleAsync(int sampleId );
      {% if cookiecutter.include_audit == "yes"%}
      Task <SampleDefinition> UpdateSampleAsync(  Sample existing, int sampleId, string name, string description );
      {% else %}
      Task UpdateSampleAsync( int sampleId, string name, string description );
      {% endif %}
     
   

   {% elif cookiecutter.database == "MongoDb" %}
     Task<string> CreateSampleAsync( Sample sample );
     Task<SampleDefinition> GetSampleAsync(string sampleId );
      {% if cookiecutter.include_audit == "yes"%}
      Task<SampleDefinition> UpdateSampleAsync(FilterDefinition<Sample> existing, string sampleId, string name, string description );
      {% else %}
      Task UpdateSampleAsync(string sampleId, string name, string description );
      {% endif %}
   {% endif %}
}
