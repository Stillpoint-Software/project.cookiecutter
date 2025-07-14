using FluentValidation;

namespace {{ cookiecutter.assembly_name }}.Core.Validators;

public interface IValidatorProvider
{
    IValidator<TPlugin> For<TPlugin>()
        where TPlugin : class;
}
