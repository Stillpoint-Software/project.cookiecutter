namespace {{cookiecutter.assembly_name}}.Data.Abstractions;

[AttributeUsage(AttributeTargets.All)]
public class Secure : Attribute
{
    public Secure()
    {
    }
}
