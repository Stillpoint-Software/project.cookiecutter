using System.Net;
using FluentValidation.Results;

namespace {{cookiecutter.assembly_name}}.Api.Validators;

public class ForbiddenValidationFailure : ValidationFailure
{
    public ForbiddenValidationFailure( string propertyName, string errorMessage ) : base( propertyName, errorMessage )
    {
        ErrorCode = HttpStatusCode.Forbidden.ToString();
    }
}
