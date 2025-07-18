using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name }}.Infrastructure.IoC;

[AttributeUsage( AttributeTargets.Class, Inherited = false )]
public sealed class RegisterServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public RegisterServiceAttribute( ServiceLifetime lifetime = ServiceLifetime.Transient )
        => Lifetime = lifetime;
}
