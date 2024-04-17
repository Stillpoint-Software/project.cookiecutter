using FluentValidation.Results;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

public interface IGetAllSamplesCommand : ICommandFunction<Arg.Empty, IAsyncEnumerable<SampleDefinition>>;

public class GetAllSamplesCommand : ServiceCommandFunction<Arg.Empty, IAsyncEnumerable<SampleDefinition>>, IGetAllSamplesCommand
{
    private readonly ISampleService _sampleService;
    //private readonly ITenantService _tenantService;

    public GetAllSamplesCommand(
        ISampleService sampleService,
        // ITenantService tenantService,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<GetAllSamplesCommand> logger )
        : base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
        //_tenantService = tenantService;
    }

    protected override FunctionAsync<Arg.Empty, IAsyncEnumerable<SampleDefinition>> CreatePipeline()
    {
        return PipelineFactory
            .Start<Arg.Empty>()
            .WithLogging()
            .Pipe( GetAllSamplesAsync )
            .Build();
    }
    private IAsyncEnumerable<SampleDefinition> GetAllSamplesAsync( IPipelineContext context, Arg.Empty _ )
    {
        var sample = _sampleService.GetAllSamplesAsync();

        if ( sample != null )
            return sample;

        context.AddValidationResult( new ValidationFailure( nameof( sample ), "Samples do not exist" ) );
        context.CancelAfter();
        return null;
    }
}
