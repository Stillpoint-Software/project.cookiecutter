namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
public record Sample
{
    public int SampleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public int TenantId { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}