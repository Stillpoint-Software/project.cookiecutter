using MongoDB.Driver;

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;

public interface IMongoDbService
{
    IMongoCollection<T> GetCollection<T>( string name );
}
