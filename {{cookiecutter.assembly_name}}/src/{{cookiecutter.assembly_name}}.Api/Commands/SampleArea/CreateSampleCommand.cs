﻿{% if cookiecutter.include_audit =='yes'%}
using Audit.Core;
{% endif %}
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Api.Identity;
using {{cookiecutter.assembly_name}}.Data.Abstractions;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;


namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

public record CreateSample( string Name, string Description );

public interface ICreateSampleCommand : ICommandFunction<CreateSample, SampleDefinition>;

public class CreateSampleCommand : ServiceCommandFunction<CreateSample, SampleDefinition>, ICreateSampleCommand
{
    private readonly ISampleService _sampleService;
    private readonly string _user;

    public CreateSampleCommand(
        ISampleService sampleService,
        IPrincipalProvider principalProvider,
        IPipelineContextFactory pipelineContextFactory,
        ILogger<CreateSampleCommand> logger ) :
        base( pipelineContextFactory, logger )
    {
        _sampleService = sampleService;
        _user = principalProvider.GetEmail();
    }

    protected override FunctionAsync<CreateSample, SampleDefinition> CreatePipeline()
    {
        return PipelineFactory
            .Start<CreateSample>()
            .WithLogging()
            .PipeAsync( CreateSampleAsync )
            .CancelOnFailure( Validate<Sample> )
            .PipeAsync( InsertSampleAsync )
            .Build();
    }

    private async Task<Sample> CreateSampleAsync( IPipelineContext context, CreateSample sample )
    {

        return await Task.FromResult( new Sample
        {
            Name = sample.Name,
            Description = sample.Description,
        } );
    }

    private async Task<SampleDefinition> InsertSampleAsync( IPipelineContext context, Sample sample )
    {
        {% if cookiecutter.include_audit =='yes'%}
         using (AuditScope.Create( "Sample:Create", () => sample ))
        {
            sample.Id = await _sampleService.CreateSampleAsync( sample );

            var sDefinition = new SampleDefinition
            (
                sample.Id,
                sample.Name,
                sample.Description
            );
            return sDefinition;
        }
        {% else %}
        sample.Id = await _sampleService.CreateSampleAsync( sample );

        return new SampleDefinition(
            sample.Id,
            sample.Name,
            sample.Description
        );
        {% endif %}
    }
}
