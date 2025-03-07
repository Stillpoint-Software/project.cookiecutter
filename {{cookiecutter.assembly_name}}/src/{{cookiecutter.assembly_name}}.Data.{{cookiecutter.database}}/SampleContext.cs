using System.Text.Json;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

public class SampleContext : DbContext
{
    private readonly JsonSerializerOptions _jsonOptions;

    public DbSet<Sample> Sample { get; set; }

     public SampleContext( DbContextOptions<SampleContext> options, IOptions<CustomJsonOptions> jsonOptions ) : base( options )
    {
        _jsonOptions = jsonOptions?.Value?.SerializerOptions ?? new JsonSerializerOptions();
    }
    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "sample" );

        var sampleTableBuilder = modelBuilder
            .Entity<Sample>()
            .ToTable( "sample" );
        sampleTableBuilder.HasKey( x => x.Id );
        sampleTableBuilder.Property( x => x.Id )
            .UseIdentityAlwaysColumn()
            .HasColumnName( "id" );
        sampleTableBuilder.Property( x => x.Name ).HasColumnName( "name" );
        sampleTableBuilder.Property( x => x.Description ).HasColumnName( "description" );
        sampleTableBuilder.Property( x => x.CreatedBy ).HasColumnName( "created_by" );
        sampleTableBuilder.Property( x => x.CreatedDate ).HasColumnName( "created_date" );
    }
}

public class CustomJsonOptions
{
    public JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions();
}
