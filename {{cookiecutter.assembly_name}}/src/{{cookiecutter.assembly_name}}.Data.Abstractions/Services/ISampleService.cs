using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
public interface ISampleService
{
    
    Task<int> CreateSampleAsync( Sample sample );
    Task<Sample> UpdateSampleAsync( int sampleId, string name, string description );
    IAsyncEnumerable<SampleDefinition> GetAllSamplesAsync( );
    Task<SampleDefinition> GetSampleAsync(int sampleId );

}
