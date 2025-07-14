using FluentValidation.Results;
using Hyperbee.Pipeline.Context;
using {{ cookiecutter.assembly_name }}.Core.Extensions;
using {{ cookiecutter.assembly_name }}.Core.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace {{ cookiecutter.assembly_name }}.Core.Commands.Middleware;

public static class PipelineContextUserExtensions
{
    public static string GetUserEmail( this IPipelineContext context )
    {
        var principalProvider = context.ServiceProvider.GetService<IPrincipalProvider>();

        var email = principalProvider?.GetEmail();
        
        if ( email != null )
            return email;

        context.AddValidationResult( new ValidationFailure( "User", "Invalid User" ) );
        context.CancelAfter();

        return null;
    }
}
