using {{cookiecutter.assembly_name}}.Infrastructure;
using Microsoft.OpenApi.Models;

namespace {{cookiecutter.assembly_name}}.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger( this IServiceCollection services, IConfiguration config )
    {
        //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/Startup.cs
        {%- if cookiecutter.include_oauth == "yes" -%}
        var authorizationUrl = $"https://{config["OAuth:Domain"]}/authorize?audience={config["OAuth:Audience"]}";
        var tokenUrl = $"https://{config["OAuth:Domain"]}/oauth/token";
        {% endif %}
        services.AddSwaggerGen( c =>
        {
            c.SwaggerDoc( "v1", new OpenApiInfo { Title = "API", Version = "v1" } );
            {%- if cookiecutter.include_oauth == "yes" -%}
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
            {% endif %}
            c.DocumentFilter<HealthChecksFilter>(); // filter to generate health check endpoint
        } );

        return services;
    }
}
