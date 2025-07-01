﻿using { { cookiecutter.assembly_name} }.Infrastructure;
using Microsoft.OpenApi.Models;

namespace {{cookiecutter.assembly_name }}.Extensions;

public static class SwaggerExtensions
{
     {% if cookiecutter.include_oauth == "yes" %}
{% include '../templates/oauth/main_swagger.cs' %}
    {% else %}


    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration config)
{
    //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/test/WebSites/OAuth2Integration/Startup.cs
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
        c.DocumentFilter<HealthChecksFilter>(); // filter to generate health check endpoint
    });

    return services;
}
{% endif %}
}