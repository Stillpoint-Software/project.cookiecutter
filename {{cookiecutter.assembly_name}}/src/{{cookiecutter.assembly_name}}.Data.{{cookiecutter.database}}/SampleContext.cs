using System.Text.Json;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

public class SampleContext : DbContext
{
    private readonly JsonSerializerOptions _jsonOptions;

    public DbSet<Sample> Sample { get; set; }

    public SampleContext( DbContextOptions<SampleContext> options, IOptions<JsonOptions> jsonOptions ) : base( options )
    {
        _jsonOptions = jsonOptions?.Value.SerializerOptions;
    }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "sample" );

        var sampleTableBuilder = modelBuilder
            .Entity<Sample>()
            .ToTable( "sample" );
        sampleTableBuilder.HasKey( x => x.SampleId );
        sampleTableBuilder.Property( x => x.SampleId )
            .UseIdentityAlwaysColumn()
            .HasColumnName( "sample_id" );
        sampleTableBuilder.Property( x => x.Name ).HasColumnName( "name" );
        sampleTableBuilder.Property( x => x.Description ).HasColumnName( "description" );
        sampleTableBuilder.Property( x => x.CreatedBy ).HasColumnName( "created_by" );
        sampleTableBuilder.Property( x => x.CreatedDate ).HasColumnName( "created_date" );
    }
}
