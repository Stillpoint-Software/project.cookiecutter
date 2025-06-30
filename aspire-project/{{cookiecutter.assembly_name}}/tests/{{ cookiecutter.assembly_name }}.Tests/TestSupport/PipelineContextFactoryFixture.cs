using {{cookiecutter.assembly_name}}.Api.Identity;
using {{cookiecutter.assembly_name}}.Api.Validators;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace {{cookiecutter.assembly_name}}.Tests.TestSupport;

public class PipelineContextFactoryFixture
{
    public static IPipelineContextFactory Next(
        ServiceCollection services = default,
        IPrincipalProvider principalProvider = default,
        IValidatorProvider validatorProvider = default
    )
    {
        services ??= new ServiceCollection();
        services.AddTransient( _ => principalProvider ?? PrincipalFixture.Next() );
        services.AddTransient( _ => validatorProvider ?? Substitute.For<IValidatorProvider>() );
        return PipelineContextFactory.CreateFactory( services.BuildServiceProvider() );
    }
}
