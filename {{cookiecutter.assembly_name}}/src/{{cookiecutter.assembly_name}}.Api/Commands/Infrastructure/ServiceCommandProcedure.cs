using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;

namespace {{cookiecutter.assembly_name}}.Api.Infrastructure;

public abstract class ServiceCommandProcedure<TOutput>(
    IPipelineContextFactory pipelineContextFactory,
    ILogger logger )
    : CommandProcedure<TOutput>( pipelineContextFactory, logger );