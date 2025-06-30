using FluentValidation;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;

namespace {{cookiecutter.assembly_name}}.Api.Validators;

public class SampleValidation : AbstractValidator<Sample>
{
    public SampleValidation()
    {
        RuleFor( x => x.Name )
            .NotEmpty()
            .NotNull()
            .WithMessage( $"{nameof( Sample.Name )} cannot be null or empty." );

    }
}
