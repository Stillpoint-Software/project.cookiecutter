
using {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet( "sample" )]
    public async Task<IActionResult> GetAllSamplesAsync(
        [FromServices] IGetAllSamplesCommand command,
        CancellationToken cancellationToken = default )
    {
        var result = await command.ExecuteAsync( cancellationToken );
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
}

public record SampleRequest( string Name, string Description, int TenantId )
{
    public CreateSample ToCommand() => new( Name, Description, TenantId );
    public UpdateSample ToCommand( int id ) => new( id, Name, Description );
}
