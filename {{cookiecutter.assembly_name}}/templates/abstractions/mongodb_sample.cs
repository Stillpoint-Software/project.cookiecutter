private string _description;

[Key]
public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
public required string Name { get; set; }
{% if cookiecutter.include_audit %}
[Secure]
public required string Description
{
    get => _description != null ? SecurityHelper.DecryptValue(_description) : "";
    set => _description = value != null ? SecurityHelper.EncryptValue(value) : "";
}
{% else %}
public required string Description { get; set; }
{% endif %}
public required string CreatedBy { get; set; }
public string CreatedDate { get; set; } = DateTimeOffset.UtcNow.ToString();