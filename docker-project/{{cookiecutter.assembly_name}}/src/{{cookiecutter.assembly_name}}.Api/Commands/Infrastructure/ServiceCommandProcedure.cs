using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;

public abstract class ServiceCommandProcedure<TOutput>(
    IPipelineContextFactory pipelineContextFactory,
    ILogger logger )
    : CommandProcedure<TOutput>( pipelineContextFactory, logger );