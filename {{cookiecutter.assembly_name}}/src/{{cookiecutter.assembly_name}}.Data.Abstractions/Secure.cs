namespace NewAspireAppAudit.Data.Abstractions;

[AttributeUsage(AttributeTargets.All)]
public class Secure : Attribute
{
    public Secure()
    {
    }
}
