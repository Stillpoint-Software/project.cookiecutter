using FluentValidation;

namespace {{cookiecutter.assembly_name}}.Api.Validators;

public interface IValidatorProvider
{
    IValidator<TPlugin> For<TPlugin>()
        where TPlugin : class;
}
