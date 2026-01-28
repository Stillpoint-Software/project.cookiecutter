using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Hyperbee.Pipeline;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;
using System;
using {{ cookiecutter.assembly_name }}.Core.Validators;
using {{ cookiecutter.assembly_name }}.Infrastructure.Configuration;
using {{ cookiecutter.assembly_name }}.Infrastructure.Extensions;
using {{ cookiecutter.assembly_name }}.Infrastructure.Data;
using {{ cookiecutter.assembly_name }}.Infrastructure.IoC;
{% if cookiecutter.database == "PostgreSql" %}
using {{ cookiecutter.assembly_name }}.Data.PostgreSql;
{% elif cookiecutter.database == "MongoDb" %}
using {{cookiecutter.assembly_name}}.Data.MongoDb;
{% endif %}

{% if cookiecutter.include_oauth %}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net.Http.Headers;
{% endif %}
{% if cookiecutter.include_azure_key_vault %}
using AzureKeyVaultEmulator.Aspire.Client;
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

        {% if cookiecutter.include_oauth %}
        //Authorization and Authentication
        services.AddAuthentication(options => //BF review hyperbee AddSecurity implementation
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration["OAuth_Domain"]}/"; // Issuer
            options.Audience = builder.Configuration["OAuth_Audience"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });

        services.AddAuthorization();

        // Configure AuthSettings from environment variables
        services.Configure<AuthSettings>(options =>
        {
            options.Domain = builder.Configuration["OAuth_Domain"];
            options.Audience = builder.Configuration["OAuth_Audience"];
            options.ClientId = builder.Configuration["OAuth_Api_ClientId"];
            options.ClientSecret = builder.Configuration["OAuth_Api_ClientSecret"];
        });
        {% endif %}

        {% if cookiecutter.include_azure_key_vault %}
        //Azure Key Vault integration
        var vaultUri = builder.Configuration.GetConnectionString("keyvault");
        if (vaultUri != null && vaultUri.Contains("localhost", StringComparison.OrdinalIgnoreCase))
        {
            //Integration using emulator with Secrets, keys, only implementation
            builder.Services.AddAzureKeyVaultEmulator(vaultUri, secrets: true, keys: true, certificates: false);
        }
        else
        {
            builder.Configuration.AddAzureKeyVaultSecrets(connectionName: "keyvault");
        }
        {% endif %}

        {% if cookiecutter.include_azure_storage %}
        //Azure Blob Storage
        builder.AddAzureBlobServiceClient("test");
        {% endif %}

        // Add Pipeline and Proxy Service
        services.AddPipeline((factoryServices, rootProvider) =>
        {
            factoryServices.ProxyService<IValidatorProvider>(rootProvider);
        });

        // Add Swagger for API documentation
        services.AddEndpointsApiExplorer(); //This has to go first
        {% if cookiecutter.include_oauth %}
        services.AddSwagger(builder.Configuration);
        {% else %}
        services.AddSwaggerGen();
        {% endif %}

        // Add services to the container before calling Build
        builder.Services.AddProblemDetails();

        {% if cookiecutter.include_audit %}
        // Configure audit setup
        AuditSetup.ConfigureAudit(builder);
        {% endif %}
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();
        app.UseSwagger(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
        });

        {% if cookiecutter.include_oauth %}
        app.UseAuthentication();
        app.UseAuthorization();
        {% endif %}

        // Use appropriate middleware based on the environment
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "{{cookiecutter.assembly_name}} API v1");
               c.RoutePrefix = string.Empty;
               {% if cookiecutter.include_oauth %}
               c.OAuthAppName(app.Configuration["OAuth_Api_AppName"]);
               c.OAuthScopeSeparator(" ");
               c.OAuthUsePkce();

               // preset id and secret in dev
               c.OAuthClientId(app.Configuration["OAuth_Swagger_Id"]);
               c.OAuthClientSecret(app.Configuration["OAuth_Swagger_Secret"]);
               {% endif %}
           });
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