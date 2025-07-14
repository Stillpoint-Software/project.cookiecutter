using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Infrastructure.Configuration;

public interface IStartupRegistry
{
    void ConfigureServices( WebApplicationBuilder builder, IServiceCollection services );
    void ConfigureScanner( ServiceRegistry services );
    void ConfigureApp( WebApplication app, IWebHostEnvironment env );
}
