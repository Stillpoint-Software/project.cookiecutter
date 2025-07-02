using FluentValidation;
using Hyperbee.Extensions.Lamar;
using Lamar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name }}.Api;

public class Startup : IStartupRegistry
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureContainer(ServiceRegistry services)
    {
        // auto-registrations by convention

        services.Scan(_ =>
        {
            _.TheCallingAssembly();
            _.WithDefaultConventions();
            _.ConnectImplementationsToTypesClosing(typeof(IValidator<>), ServiceLifetime.Transient);
        });

        services.AddHttpContextAccessor();

        // IOptions<T>

        // include implementation registries 
        services.IncludeStartupRegistry < Data.{{ cookiecutter.database }}.Startup > (Configuration);

        // additional registrations
        services.AddBackgroundServices();
        services.Configure<ApiSettings>(options => Configuration.GetSection("Api").Bind(options));

        {% if cookiecutter.include_azure == "yes" %}
        services.Configure<AzureStorageSettings>(options =>
        {
            options.ConnectionString = Configuration.GetValue("Azure:Storage:Data:ConnectionString", "");
            options.ContainerName = Configuration.GetValue("Azure:Storage:ContainerName", "");
        });
        {% endif %}
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

public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationHttpMessageHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }


    private async Task<string> GetAccessTokenAsync()
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
