using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace {{cookiecutter.assembly_name}}.Infrastructure;

public class HealthChecksFilter : IDocumentFilter
{
    public const string HealthCheckEndpoint = @"/api/Health";

    public void Apply( OpenApiDocument openApiDocument, DocumentFilterContext context )
    {
        // add an api entry for the health check

        var pathItem = new OpenApiPathItem();
        var operation = new OpenApiOperation();

        operation.Tags.Add( new OpenApiTag { Name = "Health" } );

        var properties = new Dictionary<string, OpenApiSchema>
        {
            { "status", new OpenApiSchema { Type = "string" } },
            { "errors", new OpenApiSchema { Type = "array" } }
        };

        var response = new OpenApiResponse();

        response.Content.Add( "application/json", new OpenApiMediaType

        {
            Schema = new OpenApiSchema
            {

                Type = "object",
                AdditionalPropertiesAllowed = true,
                Properties = properties,
            }
        } );

        operation.Responses.Add( "200", response );
        pathItem.AddOperation( OperationType.Get, operation );
        openApiDocument?.Paths.Add( HealthCheckEndpoint, pathItem );
    }
}
