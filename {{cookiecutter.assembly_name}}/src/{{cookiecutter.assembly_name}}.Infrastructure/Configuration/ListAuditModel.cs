namespace {{cookiecutter.assembly_name}}.Infrastructure;

public class ListAuditModel
{
    {% if cookiecutter.database =="MongoDb" %}
    public string Id { get; set; }
    {% else %}
    public int Id { get; set; }
   {% endif %} 
}
