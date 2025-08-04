using System.Net;
using FluentValidation.Results;

namespace {{ cookiecutter.assembly_name }}.Core.Validators;

public class ForbiddenValidationFailure : ValidationFailure
{
    public ForbiddenValidationFailure( string propertyName, string errorMessage ) : base( propertyName, errorMessage )
    {
        ErrorCode = nameof(HttpStatusCode.Forbidden);
    }
}
