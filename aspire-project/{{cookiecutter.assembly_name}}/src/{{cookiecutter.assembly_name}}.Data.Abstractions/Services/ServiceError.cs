namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;

public class ServiceError
{
    public string? Message { get; init; }
    public int Code { get; init; }
    public string? Name { get; init; }
    public SeverityLevel Severity { get; init; }

    public ServiceError()
    {
    }

    public ServiceError( string message )
    {
        Message = message;
    }
}