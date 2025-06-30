using FluentValidation.Results;

namespace {{cookiecutter.assembly_name}}.Api.Validators;

public class CustomValidationFailure : ValidationFailure
{
    public CustomValidationFailure( string propertyName, string errorMessage, string errorCode ) : base( propertyName, errorMessage, errorCode )
    {
        ErrorCode = errorCode;
    }
}
