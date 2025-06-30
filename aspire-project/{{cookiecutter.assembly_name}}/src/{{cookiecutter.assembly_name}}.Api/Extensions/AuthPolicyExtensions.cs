using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Api.Extensions;

internal static class AuthPolicyExtensions
{
    public static IServiceCollection AddAuthorizationPolicies( this IServiceCollection services )
    {
        return services;
    }
}