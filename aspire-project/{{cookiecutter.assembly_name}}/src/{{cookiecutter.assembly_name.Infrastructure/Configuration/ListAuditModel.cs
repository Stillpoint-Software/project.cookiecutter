namespace {{cookiecutter.assembly_name}}.Api.Infrastructure;

public class ListAuditModel
{
    {% if cookiecutter.database =="MongoDb" %}
    public string Id { get; set; }
    {% else %}
    public int Id { get; set; }
   {% endif %} 
}
