using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;

namespace {{cookiecutter.assembly_name}}.Api.Infrastucture;

public abstract class ServiceCommandProcedure<TOutput>(
    IPipelineContextFactory pipelineContextFactory,
    ILogger logger )
    : CommandProcedure<TOutput>( pipelineContextFactory, logger );