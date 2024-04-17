using System.Reflection;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;
using {{cookiecutter.assembly_name}}.Api.Validators;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace {{cookiecutter.assembly_name}}.Api.Controllers;

public class ServiceControllerBase : ControllerBase
{
    [NonAction]
    public IActionResult CommandResponse<TOutput>( CommandResult<TOutput> commandResult, Func<CommandResult<TOutput>, IActionResult> responseAction = default )
    {
        if ( TryHandleInvalidCommand( commandResult.Context, commandResult.CommandType, out var errorResult ) )
            return errorResult;

        return responseAction?.Invoke( commandResult ) ?? Ok( commandResult.Result );
    }

    [NonAction]
    public IActionResult CommandResponse( CommandResult commandResult, Func<CommandResult, IActionResult> responseAction = default )
    {
        if ( TryHandleInvalidCommand( commandResult.Context, commandResult.CommandType, out var errorResult ) )
            return errorResult;

        return responseAction?.Invoke( commandResult ) ?? Ok();
    }

    [NonAction]
    public IActionResult CommandResponse( CommandResult<Stream> commandResult, string contentType = "application/json" )
    {
        if ( TryHandleInvalidCommand( commandResult.Context, commandResult.CommandType, out var errorResult ) )
            return errorResult;

        return File( commandResult.Result, contentType );   // BF netcore 7 now allows Result.Stream. Should we be using that?
    }

    // validation handling

    protected virtual bool TryHandleInvalidCommand( IPipelineContext context, MemberInfo commandType, out IActionResult errorResult )
    {
        if ( !context.IsValid() )
        {
            errorResult = new ObjectResult( "" );

            if ( context.ValidationFailures().OfType<Commands.Infrastucture.ForbiddenValidationFailure>().Any() )
            {
                errorResult = Forbid();

                return true;
            }
            if ( context.ValidationFailures().OfType<UnauthorizedValidationFailure>().Any() )
            {
                errorResult = Unauthorized();

                return true;
            }
            if ( context.ValidationFailures().OfType<CustomValidationFailure>().Any() )
            {
                errorResult = BadRequest( context.ValidationFailures().OfType<CustomValidationFailure>().Select( x => new
                {
                    x.ErrorCode,
                    x.ErrorMessage
                } ) );

                return true;
            }

            errorResult = BadRequest( context.ValidationFailures().Select( x => new
            {
                x.PropertyName,
                x.ErrorMessage,
                x.ErrorCode
            } ) );


            context.Logger?.LogInformation( "Command [name={Name}, action={Action}, validationResult={@ValidationResult}]", commandType.Name, "Validation", errorResult );

            return true;
        }

        if ( context.IsCanceled && context.CancellationValue is IActionResult cancellationValue )
        {
            errorResult = cancellationValue;

            context.Logger?.LogInformation( "Command [name={Name}, action={Action}, cancellationResult={@CancellationResult}]", commandType.Name, "Canceled", errorResult );
            return true;
        }

        if ( context.IsError )
            throw new CommandException( $"Command {commandType.Name} failed", context.Exception );

        errorResult = null;
        return false;
    }
}
