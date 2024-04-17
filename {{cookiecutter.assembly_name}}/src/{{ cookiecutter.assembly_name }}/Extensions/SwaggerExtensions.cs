using {{cookiecutter.assembly_name}}.Infrastructure;
using Microsoft.OpenApi.Models;

namespace {{cookiecutter.assembly_name}}.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger( this IServiceCollection services, IConfiguration config )
    {
        //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/Startup.cs

        var authorizationUrl = $"https://{config["OAuth:Domain"]}/authorize?audience={config["OAuth:Audience"]}";
        var tokenUrl = $"https://{config["OAuth:Domain"]}/oauth/token";

        services.AddSwaggerGen( c =>
        {
            c.SwaggerDoc( "v1", new OpenApiInfo { Title = "API", Version = "v1" } );

            c.AddSecurityDefinition( "oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,

                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri( authorizationUrl ),
                        TokenUrl = new Uri( tokenUrl )
                    }
                }
            } );

            c.AddSecurityRequirement( new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[]
                    {
                        "admin:read",
                        "admin:write"
                    }
                }
            } );

            c.OperationFilter<SecurityRequirementsOperationFilter>();

            c.DocumentFilter<HealthChecksFilter>(); // filter to generate health check endpoint
        } );

        return services;
    }
}
