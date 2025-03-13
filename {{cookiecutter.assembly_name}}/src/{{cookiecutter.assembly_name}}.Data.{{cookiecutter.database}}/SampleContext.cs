using System.Data;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using Microsoft.EntityFrameworkCore;


namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};

public class SampleContext : DbContext
{

    {% if cookiecutter.include_audit == 'yes' %}
    private readonly string encryptionKey = "mysecretkey"; // use azure key vault for this
    {% endif %}

     public DbSet<Sample> Sample { get; set; }

     public SampleContext( DbContextOptions<SampleContext> options ) : base( options )
    {
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
        {% if cookiecutter.include_audit == 'yes' %}
                sampleTableBuilder.Property( x => x.Description ).HasColumnName( "description" )
          .HasColumnType( "bytea" )
            .HasConversion(
                val => EncryptData( val ?? string.Empty ),
                val => DecryptData( val ) ); 
        {% else %}
        sampleTableBuilder.Property( x => x.Description ).HasColumnName( "description" );
        {% endif %}
        sampleTableBuilder.Property( x => x.CreatedBy ).HasColumnName( "created_by" );
        sampleTableBuilder.Property( x => x.CreatedDate ).HasColumnName( "created_date" );
    }
    {% if cookiecutter.include_audit == 'yes' and cookiecutter.database == 'PostgreSql' %}
     {% include '/templates/audit/data.postgresql.encryption.cs' %}
    {% endif %}
}
