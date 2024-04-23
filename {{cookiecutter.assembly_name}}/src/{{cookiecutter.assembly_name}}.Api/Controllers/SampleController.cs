
using {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
{%- if cookiecutter.database == "Mongo" -%}
using MongoDB.Bson;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Api.Controllers;

[ApiController]
[Route( "api/[controller]" )]
[Asp.Versioning.ApiVersion( "1.0" )]
[Authorize]
public class SampleController : ServiceControllerBase
{
    [HttpPost( "sample" )]
    public async Task<IActionResult> CreateSampleAsync(
        [FromServices] ICreateSampleCommand command,
        [FromBody] SampleRequest request,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( request.ToCommand(), cancellationToken );
        return CommandResponse( result );
    }
   {%- if cookiecutter.database == "Postgresql" -%}
    [HttpGet( "sample/{sampleId:int}" )]
    public async Task<IActionResult> GetSampleAsync(
        [FromServices] IGetSampleCommand command,
        [FromRoute] int sampleId,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( sampleId, cancellationToken );
        return CommandResponse( result );
    }

    [HttpPut( "sample/{sampleIdId:int}" )]
    public async Task<IActionResult> UpdateSampleAsync(
        [FromServices] IUpdateSampleCommand command,
        [FromRoute] int sampleId,
        [FromBody] SampleRequest request,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( request.ToCommand( sampleId ), cancellationToken );
        return CommandResponse( result );
    }
    {%- elif cookiecutter.database == "Mongo" -%}
     [HttpGet( "sample/{sampleId:string}" )]
    public async Task<IActionResult> GetSampleAsync(
        [FromServices] IGetSampleCommand command,
        [FromRoute] string sampleId,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( sampleId, cancellationToken );
        return CommandResponse( result );
    }

    [HttpPut( "sample/{sampleId:string}" )]
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
    {%- if cookiecutter.database == "Postgresql" -%}
    public UpdateSample ToCommand( int id ) => new( id, Name, Description );
    {%- elif cookiecutter.database == "Mongo" -%}
    public UpdateSample ToCommand( string id ) => new( id, Name, Description );
    {% endif %}
}
