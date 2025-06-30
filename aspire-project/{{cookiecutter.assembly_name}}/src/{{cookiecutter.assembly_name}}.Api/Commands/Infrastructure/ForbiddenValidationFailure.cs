using FluentValidation.Results;
using System.Net;

namespace {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;

public class ForbiddenValidationFailure : ValidationFailure
{
    public ForbiddenValidationFailure( string propertyName, string errorMessage ) : base( propertyName, errorMessage )
    {
        ErrorCode = HttpStatusCode.Forbidden.ToString();
    }
}
