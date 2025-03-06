using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
public interface ISampleService
{
   {% if cookiecutter.database == "PostgreSql" %}
    Task<int> CreateSampleAsync( Sample sample );
    Task UpdateSampleAsync( int sampleId, string name, string description );
    Task<SampleDefinition> GetSampleAsync(int sampleId );
   {% elif cookiecutter.database == "MongoDb" %}
   Task CreateSampleAsync( Sample sample );
   Task UpdateSampleAsync( string sampleId, string name, string description );
   Task<SampleDefinition> GetSampleAsync( string sampleId );
   {% endif %}
}
