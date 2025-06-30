namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

[AttributeUsage( AttributeTargets.Class, Inherited = false )]
public class BsonCollectionAttribute( string collectionName ) : Attribute
{
    public string CollectionName { get; } = collectionName;
}
