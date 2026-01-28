using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using {{ cookiecutter.assembly_name }}.Infrastructure.Extensions;
using {{ cookiecutter.assembly_name }}.Api.Commands.SampleArea;

namespace {{cookiecutter.assembly_name }}.Api.Endpoints;

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

        {% if cookiecutter.database == "PostgreSql" %}
        {% include 'templates/api/postgresql_sample_endpoints.cs' %}
        {% endif %}
        {% if cookiecutter.database == "MongoDb" %}
        {% include 'templates/api/mongodb_sample_endpoints.cs' %}
        {% endif %}

        return group;
    }

    public record SampleRequest(string Name, string Description)
    {
        public CreateSample ToCommand() => new(Name, Description);
        {% if cookiecutter.database == "PostgreSql" %}
    public UpdateSample ToCommand(int sampleId) => new(sampleId, Name, Description);
        {% endif %}
{% if cookiecutter.database == "MongoDb" %}
public UpdateSample ToCommand(string sampleId) => new(sampleId, Name, Description);
{% endif %}
    }
}
