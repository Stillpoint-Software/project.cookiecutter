using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
public interface ISampleService
{
   {%- if cookiecutter.database == "Postgresql" -%}
    Task<int> CreateSampleAsync( Sample sample );
    Task UpdateSampleAsync( int sampleId, string name, string description );
    Task<SampleDefinition> GetSampleAsync(int sampleId );
   {%- elif cookiecutter.database == "Mongo" -%}
   Task CreateSampleAsync( Sample sample );
   Task UpdateSampleAsync( string sampleId, string name, string description );
   Task<SampleDefinition> GetSampleAsync( string sampleId );
   {% endif %}
  

}
