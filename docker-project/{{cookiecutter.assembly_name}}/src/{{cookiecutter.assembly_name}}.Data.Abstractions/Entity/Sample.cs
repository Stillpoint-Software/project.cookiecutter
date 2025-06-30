{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
{% endif %}
namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
public record Sample
{
   {% if cookiecutter.database == "PostgreSql" %}
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string CreatedBy { get; set; }
    public DateTimeOffset? CreatedDate { get; set; } = DateTimeOffset.UtcNow;
   {% elif cookiecutter.database == "MongoDb" %}
    [BsonId]
    [BsonRepresentation( BsonType.ObjectId )]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public required string Name { get; set; }
    public required string Description { get; set; }
    [BsonElement( "created_by" )]
    public required string CreatedBy { get; set; }
    [BsonElement( "created_date" )]
    [BsonRepresentation( BsonType.String )]
    public DateTimeOffset? CreatedDate { get; set; } = DateTimeOffset.UtcNow;
   {% endif %}
}