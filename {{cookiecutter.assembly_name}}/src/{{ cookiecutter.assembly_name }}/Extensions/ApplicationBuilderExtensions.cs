using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace {{cookiecutter.assembly_name}}.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationStopped( this IApplicationBuilder app,
        IHostApplicationLifetime applicationLifetime, Action action )
    {
        applicationLifetime.ApplicationStopped.Register( action );
        return app;
    }
}
