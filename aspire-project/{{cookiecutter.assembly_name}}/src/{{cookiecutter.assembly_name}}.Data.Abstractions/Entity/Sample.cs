{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
{% endif %}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using {{cookiecutter.assembly_name}}.Core.Security;


namespace {{cookiecutter.assembly_name }}.Data.Abstractions.Entity;
public record Sample
{
   {% if cookiecutter.database == "PostgreSql" %}
   [Key]
   public int Id { get; set; }
    public required string Name { get; set; }
   [Secure]
   [Column(TypeName = "bytea")]
    public required string Description { get; set; }
    public required string CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
   {% elif cookiecutter.database == "MongoDb" %}
   private string _description;

   [Key]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public required string Name { get; set; }
   [Secure]
   public required string Description
   {
      get => _description != null ? SecurityHelper.DecryptValue(_description) : null;
      set => _description = value != null ? SecurityHelper.EncryptValue(value) : null;
   }
    public required string CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow.ToString();
   {% endif %}
}