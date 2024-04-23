using MongoDB.Driver;

namespace {{cookiecutter.assembly_name}}.Migrations.System;

internal interface IMigrationRepository
{
    Task<IList<MigrationRecord>> GetMigrationRecordsAsync();

    Task ApplyMigrationAsync( MigrationRecord record );
    Task RemoveMigrationAsync( MigrationRecord record );
}

internal class MigrationRepository : IMigrationRepository
{
    private readonly IMongoCollection<MigrationRecord> _collection;

    public MigrationRepository( IMongoDatabase database )
    {
        _collection = database.GetCollection<MigrationRecord>( nameof( MigrationRecord ) );
    }

    public async Task<IList<MigrationRecord>> GetMigrationRecordsAsync()
    {
        // Need to have an index with any sorting when using Azure MongoDB API for CosmoDB
        await CreateMigrationIndexAsync();
        return await _collection
            .Find( Builders<MigrationRecord>.Filter.Empty )
            .SortByDescending( x => x.Version )
            .ToListAsync();
    }

    public async Task ApplyMigrationAsync( MigrationRecord record )
    {
        await _collection.InsertOneAsync( record );
    }

    public async Task RemoveMigrationAsync( MigrationRecord record )
    {
        await _collection.DeleteOneAsync( x => x.Version == record.Version );
    }

    public async Task CreateMigrationIndexAsync()
    {
        var indexBuilder = Builders<MigrationRecord>.IndexKeys;
        var keys = indexBuilder.Ascending( "Version" );
        var options = new CreateIndexOptions
        {
            Name = "Version"
        };
        var indexModel = new CreateIndexModel<MigrationRecord>( keys, options );
        await _collection.Indexes.CreateOneAsync( indexModel, cancellationToken: default );
    }
}
