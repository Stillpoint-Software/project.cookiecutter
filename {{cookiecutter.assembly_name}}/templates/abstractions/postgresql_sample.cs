public int Id { get; set; }
public required string Name { get; set; }
{%- if cookiecutter.include_audit == "yes" %}
[Secure]
[Column(TypeName = "bytea")]
{%- endif %}
public required string Description { get; set; }
public required string CreatedBy { get; set; }
public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;