using FluentValidation;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{cookiecutter.assembly_name}}.Api.Validators;

namespace {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;

public abstract class ServiceCommandFunction<TInput, TOutput>(
    IPipelineContextFactory pipelineContextFactory,
    ILogger logger )
    : CommandFunction<TInput, TOutput>( pipelineContextFactory, logger )
{
    public static IValidator Validate<T>( IValidatorProvider provider ) where T : class => provider.For<T>();
}

public abstract class ServiceCommandFunction<TOutput>(
    IPipelineContextFactory pipelineContextFactory,
    ILogger logger )
    : CommandFunction<TOutput>( pipelineContextFactory, logger );
