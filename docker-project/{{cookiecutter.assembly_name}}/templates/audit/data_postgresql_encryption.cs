 public byte[] EncryptData( string text )
    {
        using var command = this.Database.GetDbConnection().CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT db_sym_encrypt(@t,@k)";

        command.Parameters.Add(
            new Npgsql.NpgsqlParameter( "t", NpgsqlTypes.NpgsqlDbType.Text ) { Value = text } );
        command.Parameters.Add(
            new Npgsql.NpgsqlParameter( "k", NpgsqlTypes.NpgsqlDbType.Text ) { Value = encryptionKey } );

        if (command.Connection?.State == ConnectionState.Closed)
        {
            command.Connection.Open();
        }

        var encrypted = (byte[]?) command.ExecuteScalar();

        return encrypted ?? throw new InvalidOperationException( "Encryption failed, result is null." );
    }

    public string DecryptData( byte[] cipher )
    {
        using var command = this.Database.GetDbConnection().CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT db_sym_decrypt(@t, @k)";

        command.Parameters.Add(
            new Npgsql.NpgsqlParameter( "t", NpgsqlTypes.NpgsqlDbType.Bytea ) { Value = cipher } );

        command.Parameters.Add(
            new Npgsql.NpgsqlParameter( "k", NpgsqlTypes.NpgsqlDbType.Text ) { Value = encryptionKey } );


        if (command.Connection?.State == ConnectionState.Closed)
        {
            command.Connection.Open();
        }

        var decrypted = (string?) command.ExecuteScalar();

        return decrypted ?? throw new InvalidOperationException( "Decryption failed, result is null." );

    }