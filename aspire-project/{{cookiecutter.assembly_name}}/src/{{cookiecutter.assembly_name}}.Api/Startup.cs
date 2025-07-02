using Asp.Versioning;
using Hyperbee.Pipeline;
{% if cookiecutter.include_oauth == "yes" %}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.Security.Claims;
{% endif %}
using {{ cookiecutter.assembly_name}}.Api.Validators;
using {{ cookiecutter.assembly_name}}.Api.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lamar;

namespace {{cookiecutter.assembly_name }}.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(c => c.AddPolicy("CorsAllowAll", build =>
        {
            build.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));

        services.AddHttpContextAccessor();

        services.AddHttpClient();

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ApiVersion.Default;
            //Add X-Version in Header
            options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        });

     
        services.AddDataProtection();

        {% if cookiecutter.include_oauth == "yes" %}
        services.AddAuthentication(options => //BF review hyperbee AddSecurity implementation
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{Configuration["OAuth:Domain"]}/"; // Issuer
            options.Audience = Configuration["OAuth:Audience"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });

        services.AddAuthorization();
        {% endif %}

        // Add Pipeline and Proxy Service
        services.AddPipeline((factoryServices, rootProvider) =>
        {
            factoryServices.ProxyService<IValidatorProvider>(rootProvider);
        });

        // Add Swagger for API documentation
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // additional registrations
        services.AddBackgroundServices();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        // Use appropriate middleware based on the environment
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "{{cookiecutter.assembly_name}} API v1");
                c.RoutePrefix = string.Empty;  // Makes Swagger UI available at the root ("/")

            });
        }
        else
        {
            app.UseHsts();
            // General middleware setup
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(c => // must be called before UseResponseCaching
            {
                c.AllowAnyOrigin();
                c.AllowAnyMethod();
                c.AllowAnyHeader();
            });

            app.UseAuthorization();
        }

        app.MapControllers();
    }

}
public static class StartupExtensions
{
    public static void AddBackgroundServices(this IServiceCollection _)
    {
        /* example
        services.Configure<HeartbeatServiceOptions>( x =>
        {
            x.PeriodSeconds = 10;
            x.Text = "ka-thump";
        } );

        services.AddHostedService<HeartbeatService>();       
         */
    }
}
{% if cookiecutter.include_oauth == "yes" %}
public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationHttpMessageHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }


    private async Task<string?> GetAccessTokenAsync()
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var authenticateResult = await httpContext.AuthenticateAsync();

        return authenticateResult.Succeeded
            ? authenticateResult.Properties.GetTokenValue("access_token")
            : null;
    }
}
{% endif %}



