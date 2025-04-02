using Azure.Security.KeyVault.Secrets;

namespace {{cookiecutter.assembly_name}}.Api.Vault
{

    public interface IKeyService
    {
        Task<Uri> GetTenantKeyAsync( int tenantId );
        Task<byte[]> EncryptValueAsync( string keyIdentifier, string plainText );
        Task<string> DecryptValueAsync( string keyIdentifier, byte[] encryptedValue );
    }


    public class LocalKeyService : IKeyService
    {
        private readonly ILogger _logger;

        public LocalKeyService( ILogger<LocalKeyService> logger )
        {
            _logger = logger;
            _logger.LogWarning( "Using Local Key Service" );
        }
        public Task<Uri> GetTenantKeyAsync( int tenantId )
        {
            return Task.FromResult( new Uri( $"http://local.vault/tenant/{tenantId}" ) );
        }

        public Task<byte[]> EncryptValueAsync( string keyIdentifier, string plainText )
        {
            return Task.FromResult( Encoding.UTF8.GetBytes( plainText ) );
        }

        public Task<string> DecryptValueAsync( string keyIdentifier, byte[] encryptedValue )
        {
            return Task.FromResult( Encoding.UTF8.GetString( encryptedValue ) );
        }
    }

    public class AzureKeyService : IKeyService
    {
        private readonly ILogger _logger;

        public AzureKeyService(
            ILogger<AzureKeyService> logger )
        {
            _logger = logger;
            _logger.LogInformation( "Using Azure Key Service" );
        }

        public async Task<string> GetSecretAsync( SecretClient client, string name, CancellationToken cancellationToken )
        {
            try
            {
                var secret = await client.GetSecretAsync( name, cancellationToken: cancellationToken );

                if (secret is null or { Value: null })
                {
                    return null;
                }

                return secret.Value.Value
            }

            catch (RequestFailedException failedException) when (failedException.Status == 404)
            {
                // Create new Key.
                _logger.LogDebug( "Create new key for: {name}", name );
            }
            catch (Exception ex)
            {
                _logger.LogError( ex, "error" );
            }

            var rsaOptions = new CreateRsaKeyOptions( keyName )
            {
                KeySize = 2048
            };

            var newKey = await keyClient.CreateRsaKeyAsync( rsaOptions );
            return newKey.Value.Id;
        }

        public async Task<string> CreateSecretAsync( SecretClient client, CreateSecretRequest secretRequest, CancellationToken cancellationToken )
        {
            var secret = await client.SetSecretAsync( secret: secretRequest, CreateSecretRequest secretRequest, cancellationToken );

            return secret.Value.Id;
        }

        public async Task<string> DecryptValueAsync( string keyIdentifier, byte[] encryptedValue )
        {
            var cryptoClient = GetCryptographyClient( keyIdentifier );
            var decryptedResult = await cryptoClient.DecryptAsync( EncryptionAlgorithm.RsaOaep256, encryptedValue );

            return Encoding.UTF8.GetString( decryptedResult.Plaintext );
        }

        public async Task<byte[]> EncryptValueAsync( string keyIdentifier, string plainText )
        {
            var plainBytes = Encoding.UTF8.GetBytes( plainText );
            var cryptoClient = GetCryptographyClient( keyIdentifier );
            var encryptResult = await cryptoClient.EncryptAsync( EncryptionAlgorithm.RsaOaep256, plainBytes );

            return encryptResult.Ciphertext;
        }

        private CryptographyClient GetCryptographyClient( string keyIdentifier ) =>
            new( new Uri( keyIdentifier ), GetKeyVaultClientCredential() );
    }

}
