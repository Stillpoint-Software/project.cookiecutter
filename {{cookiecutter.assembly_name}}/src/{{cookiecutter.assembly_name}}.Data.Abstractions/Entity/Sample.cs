{%- if cookiecutter.database == "Mongo" -%}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
public record Sample
{
   {%- if cookiecutter.database == "Postgresql" -%}
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
   {%- elif cookiecutter.database == "Mongo" -%}
    [BsonId]
    [BsonRepresentation( BsonType.ObjectId )]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    [BsonElement( "created_by" )]
    public string CreatedBy { get; set; }
    [BsonElement( "created_date" )]
    [BsonRepresentation( BsonType.String )]
    public DateTimeOffset? CreatedDate { get; set; } = DateTimeOffset.UtcNow;
   {% endif %}

 
}