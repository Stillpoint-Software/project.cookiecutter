using System.Data;
using {{ cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using Microsoft.EntityFrameworkCore;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
{% endif %}

namespace {{cookiecutter.assembly_name }}.Data.{{ cookiecutter.database }};

public class DatabaseContext : DbContext
{
    {% if cookiecutter.include_audit == 'yes' and cookiecutter.database =="PostgreSql" %}
    private readonly string encryptionKey = "mysecretkey"; // use azure key vault for this
    {% endif %}

public DbSet<Sample> Samples { get; set; }

public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
}

{% if cookiecutter.database == "PostgreSql" %}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("{{ cookiecutter.database_name | lower }}");

        var sampleTableBuilder = modelBuilder
            .Entity<Sample>()
            .ToTable("sample");
        sampleTableBuilder.HasKey(x => x.Id);
        sampleTableBuilder.Property(x => x.Id)
            .UseIdentityAlwaysColumn()
            .HasColumnName("id");
        sampleTableBuilder.Property(x => x.Name).HasColumnName("name");
        {% if cookiecutter.include_audit == 'yes' %}
        sampleTableBuilder.Property(x => x.Description).HasColumnName("description")
        .HasColumnType("bytea")
        .HasConversion(
            val => EncryptData(val ?? string.Empty),
            val => DecryptData(val));
        {% else %}
        sampleTableBuilder.Property(x => x.Description).HasColumnName("description");
        {% endif %}
        sampleTableBuilder.Property(x => x.CreatedBy).HasColumnName("created_by");
        sampleTableBuilder.Property(x => x.CreatedDate).HasColumnName("created_date");
    }
    {% if cookiecutter.include_audit == 'yes' and cookiecutter.database == 'PostgreSql' %}
    {% include 'templates/audit/data_postgresql_encryption.cs' %}
    {% endif %}

{% elif cookiecutter.database == "MongoDb" %}

    public static DatabaseContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<DatabaseContext>()
                                        .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                                        .Options);

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    var sampleIndex = new CreateIndexModel<Sample>(Builders<Sample>.IndexKeys
                           .Ascending(x => x.Name)
                           .Ascending(x => x.Description));

    modelBuilder.Entity<Sample>().ToCollection("sample");
}

protected override void ConfigureConventions(ModelConfigurationBuilder configBuilder)
{
    //To use camel case field names in the serialized document
    var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
    ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
}
{% endif %}
}
