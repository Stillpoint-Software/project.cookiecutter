namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;

public interface IAuthService
{
    Task<string> GetApiToken();
    Task<int> CreateUser( string name, string email );
}