using {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
namespace {{cookiecutter.assembly_name}}.Api.Controllers;

[ApiController]
[Route( "api/[controller]" )]
[ApiVersion("1.0")]
{% if cookiecutter.include_oauth=="yes" %}
[Authorize]
{% endif %}
public class SampleController : ServiceControllerBase
{
    [HttpPost("sample" )]
    public async Task<IActionResult> CreateSampleAsync(
        [FromServices] ICreateSampleCommand command,
        [FromBody] SampleRequest request,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( request.ToCommand(), cancellationToken );
        return CommandResponse( result );
    }
   {% if cookiecutter.database == "PostgreSql" %}
    [HttpGet( "{sampleId:int}" )]
    public async Task<IActionResult> GetSampleAsync(
        [FromServices] IGetSampleCommand command,
        [FromRoute] int sampleId,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( sampleId, cancellationToken );
        return CommandResponse( result );
    }

    [HttpPut( "sample/{sampleId:int}" )]
    public async Task<IActionResult> UpdateSampleAsync(
        [FromServices] IUpdateSampleCommand command,
        [FromRoute] int sampleId,
        [FromBody] SampleRequest request,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( request.ToCommand( sampleId ), cancellationToken );
        return CommandResponse( result );
    }
 {% elif cookiecutter.database == "MongoDb" %} 
     [HttpGet( "{sampleId}" )]
    public async Task<IActionResult> GetSampleAsync(
        [FromServices] IGetSampleCommand command,
        [FromRoute] string sampleId,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( sampleId, cancellationToken );
        return CommandResponse( result );
    }

    [HttpPut( "sample/{sampleId}" )]
    public async Task<IActionResult> UpdateSampleAsync(
        [FromServices] IUpdateSampleCommand command,
        [FromRoute] string sampleId,
        [FromBody] SampleRequest request,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( request.ToCommand( sampleId ), cancellationToken );
        return CommandResponse( result );
    }
   {% endif %}
}

public record SampleRequest( string Name, string Description )
{
    public CreateSample ToCommand() => new( Name, Description );
    {% if cookiecutter.database == "PostgreSql" %}
    public UpdateSample ToCommand( int sampleId ) => new( sampleId, Name, Description );
    {% elif cookiecutter.database == "MongoDb" %}
    public UpdateSample ToCommand( string sampleId ) => new( sampleId, Name, Description );
    {% endif %}
}
