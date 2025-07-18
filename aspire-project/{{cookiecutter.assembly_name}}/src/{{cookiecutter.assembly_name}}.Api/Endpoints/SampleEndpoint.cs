using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using {{cookiecutter.assembly_name}}.Infrastructure.Extensions;

namespace {{cookiecutter.assembly_name}}.Api.Endpoints;

public static class SampleEndpoints
{
    public static RouteGroupBuilder MapSampleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("api/sample")
            .WithTags("Sample")
            .WithApiVersionSet(
                endpoints.NewApiVersionSet()
                         .HasApiVersion(new ApiVersion(1, 0))
                         .Build())
            .HasApiVersion(1.0);

        group.MapPost("/", async (
            [FromServices] ICreateSampleCommand command,
            [FromBody] SampleRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await command.ExecuteAsync(request.ToCommand(), cancellationToken);
            return result.ToResult();
        });

        group.MapGet("/{sampleId:int}", async (
            [FromServices] IGetSampleCommand command,
            int sampleId,
            CancellationToken cancellationToken) =>
        {
            var result = await command.ExecuteAsync(sampleId, cancellationToken);
            return result.ToResult();
        });

        group.MapPut("/{sampleId:int}", async (
            [FromServices] IUpdateSampleCommand command,
            int sampleId,
            [FromBody] SampleRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await command.ExecuteAsync(request.ToCommand(sampleId), cancellationToken);
            return result.ToResult();
        });

        return group;
    }

    public record SampleRequest(string Name, string Description)
    {
        public CreateSample ToCommand() => new(Name, Description);

        public UpdateSample ToCommand(int sampleId) => new(sampleId, Name, Description);

    }

}
