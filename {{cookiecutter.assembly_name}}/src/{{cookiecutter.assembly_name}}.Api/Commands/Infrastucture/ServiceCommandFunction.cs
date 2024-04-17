using FluentValidation;
using {{cookiecutter.assembly_name}}.Api.Validators;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;

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
