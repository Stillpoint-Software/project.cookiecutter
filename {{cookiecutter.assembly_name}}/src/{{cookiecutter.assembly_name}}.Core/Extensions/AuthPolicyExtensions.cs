using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Core.Extensions;

internal static class AuthPolicyExtensions
{
    public static IServiceCollection AddAuthorizationPolicies( this IServiceCollection services )
    {
        return services;
    }
}