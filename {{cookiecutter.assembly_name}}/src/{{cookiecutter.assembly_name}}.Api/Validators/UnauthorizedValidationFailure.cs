using System.Net;
using FluentValidation.Results;

namespace {{cookiecutter.assembly_name}}.Api.Validators;

public class UnauthorizedValidationFailure : ValidationFailure
{
    public UnauthorizedValidationFailure( string propertyName, string errorMessage ) : base( propertyName, errorMessage )
    {
        ErrorCode = HttpStatusCode.Unauthorized.ToString();
    }
}
