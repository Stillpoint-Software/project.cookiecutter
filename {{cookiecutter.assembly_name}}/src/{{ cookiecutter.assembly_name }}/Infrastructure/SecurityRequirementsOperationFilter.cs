using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace {{cookiecutter.assembly_name}}.Infrastructure;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/ResourceServer/Swagger/SecurityRequirementsOperationFilter.cs

    public void Apply( OpenApiOperation operation, OperationFilterContext context )
    {
        // policy names map to scopes
        var requiredScopes = context.MethodInfo
            .GetCustomAttributes( true )
            .OfType<AuthorizeAttribute>()
            .Select( attr => attr.Policy )
            .Distinct()
            .ToList();

        if ( !requiredScopes.Any() )
            return;

        operation.Responses.Add( "401", new OpenApiResponse { Description = "Unauthorized" } );
        operation.Responses.Add( "403", new OpenApiResponse { Description = "Forbidden" } );

        var oauthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [oauthScheme] = requiredScopes
            }
        };
    }
}
