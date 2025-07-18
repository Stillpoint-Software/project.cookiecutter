using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Hyperbee.Pipeline;
using Lamar;
using {{cookiecutter.assembly_name}}.Core.Validators;
using {{cookiecutter.assembly_name}}.Infrastructure.Configuration;
using {{cookiecutter.assembly_name}}.Infrastructure.Data;
using {{cookiecutter.assembly_name}}.Infrastructure.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
{% if cookiecutter.include_oauth == "yes" %}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.Security.Claims;
{% endif %}

namespace {{cookiecutter.assembly_name }}.Infrastructure;

public class Startup : IStartupRegistry
{
    public void ConfigureServices(IHostApplicationBuilder builder, IServiceCollection services)
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
            options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        });

        services.AddControllers()
        .AddJsonOptions(x =>
        {
            // serialize enums as strings in api responses (e.g. Color)
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonBoolConverter());
            x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddDataProtection();

        //Authorization and Authentication
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

        //Azure Blob Storage and Key Vault
        {% if cookiecutter.include_azure_key_vault == "yes" %}
        var connectionString = builder.Configuration["ConnectionStrings:secrets"];

        if (!string.IsNullOrEmpty(connectionString))
        {
            //Add Azure Key Vault secret values to app configuration
            builder.Configuration.AddAzureKeyVaultSecrets("secrets");

            //add Azure Key Vault 'SecretClient' to DI Container
            builder.AddAzureKeyVaultClient("secrets");
        }
        {% endif %}
        {% if cookiecutter.include_azure_blob_storage == "yes" %}
        //Add Azure Blob Storage to DI Container
        builder.AddAzureBlobClient("blobs");
        {% endif %}

        // Add Pipeline and Proxy Service
        services.AddPipeline((factoryServices, rootProvider) =>
        {
            factoryServices.ProxyService<IValidatorProvider>(rootProvider);
        });

        // Add Swagger for API documentation
        services.AddEndpointsApiExplorer(); //AV this has to go first
        services.AddSwaggerGen();

        // Add services to the container before calling Build
        builder.Services.AddProblemDetails();

        // Configure Serilog setup
        SerilogSetup.ConfigureSerilog(builder);

        {% if cookiecutter.include_audit == 'yes' %}
        // Configure audit setup
        AuditSetup.ConfigureAudit(builder);
        {% endif %}
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        // Use appropriate middleware based on the environment
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
        }
        else
        {
            app.UseHsts();
            // General middleware setup
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(c => // must be called before UseResponseCaching
            {
                c.AllowAnyOrigin();
                c.AllowAnyMethod();
                c.AllowAnyHeader();
            });

            app.UseAuthorization();
        }
    }

    public void ConfigureScanner(ServiceRegistry services)
    {
        IdentityModelEventSource.ShowPII = true; // show pii info in logs for debugging openid

        services.Scan(scanner =>
        {
            scanner.TheCallingAssembly();
            scanner.WithDefaultConventions();
            scanner.WithRegisterServiceConventions();
        });
    }
}