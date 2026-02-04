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
    {%- if cookiecutter.include_audit %}
    sampleTableBuilder.Property(x => x.Description).HasColumnName("description")
    .HasColumnType("bytea")
    .HasConversion(
        val => EncryptData(val ?? string.Empty),
        val => DecryptData(val));
    {%- else %}
    sampleTableBuilder.Property(x => x.Description).HasColumnName("description");
    {%- endif %}
    sampleTableBuilder.Property(x => x.CreatedBy).HasColumnName("created_by");
    sampleTableBuilder.Property(x => x.CreatedDate).HasColumnName("created_date");
}

{%- if cookiecutter.include_audit and cookiecutter.database == 'PostgreSql' %}
public byte[] EncryptData(string text)
{
    using var command = this.Database.GetDbConnection().CreateCommand();

    command.CommandType = CommandType.Text;
    command.CommandText = "SELECT {{cookiecutter.database_name| lower}}.db_sym_encrypt(@t,@k)";

    command.Parameters.Add(
        new Npgsql.NpgsqlParameter("t", NpgsqlTypes.NpgsqlDbType.Text) { Value = text });
    command.Parameters.Add(
        new Npgsql.NpgsqlParameter("k", NpgsqlTypes.NpgsqlDbType.Text) { Value = encryptionKey });

    if (command.Connection?.State == ConnectionState.Closed)
    {
        command.Connection.Open();
    }

    var encrypted = (byte[]?)command.ExecuteScalar();

    return encrypted ?? throw new InvalidOperationException("Encryption failed, result is null.");
}

public string DecryptData(byte[] cipher)
{
    using var command = this.Database.GetDbConnection().CreateCommand();

    command.CommandType = CommandType.Text;
    command.CommandText = "SELECT {{cookiecutter.database_name| lower}}.db_sym_decrypt(@t, @k)";

    command.Parameters.Add(
        new Npgsql.NpgsqlParameter("t", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = cipher });

    command.Parameters.Add(
        new Npgsql.NpgsqlParameter("k", NpgsqlTypes.NpgsqlDbType.Text) { Value = encryptionKey });


    if (command.Connection?.State == ConnectionState.Closed)
    {
        command.Connection.Open();
    }

    var decrypted = (string?)command.ExecuteScalar();

    return decrypted ?? throw new InvalidOperationException("Decryption failed, result is null.");

}
{%- endif %}