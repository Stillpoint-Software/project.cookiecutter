using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using {{cookiecutter.assembly_name}}.Api.Identity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace {{cookiecutter.assembly_name}}.Core.Identity;
public class AuthService : IAuthService
{

    private readonly IHttpClientFactory _clientFactory;
    private readonly AuthSettings _authSettings;
    private readonly ILogger _logger;

    public AuthService( IHttpClientFactory clientFactory, IOptions<AuthSettings> authSettings, ILogger<AuthService> logger )
    {
        _clientFactory = clientFactory;
        _authSettings = authSettings.Value;
        _logger = logger;
    }
    
    public async Task<string> GetApiToken()
    {
        try
        {
            var _client = _clientFactory.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
            var baseAddress = $"https://{_authSettings.Domain}/oauth/token";

            _client.BaseAddress = new Uri( baseAddress );

            var parameters = new FormUrlEncodedContent( new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id",$"{ _authSettings.ClientId}"),
                new("client_secret", $"{ _authSettings.ClientSecret}" ),
                new("audience", $"{ _authSettings.Audience}")
            } );

            var request = new HttpRequestMessage( HttpMethod.Post, _client.BaseAddress )
            {
                Content = parameters
            };


            var response = await _client.SendAsync( request );
            if ( response.IsSuccessStatusCode )
            {
                var result = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthResult>( result );
                return authResult.AccessToken;
            }

            return null;

        }
        catch ( Exception ex )
        {
            _logger.LogError( $"Error calling [{_authSettings.Domain}/oauth/token]" );
            throw new ServiceException( nameof( GetApiToken ), "Error getting Api token", ex );
        }
    }

    public async Task<int> CreateUser( string name, string email )
    {
        try
        {
            var token = await GetApiToken();
            var _client = _clientFactory.CreateClient();

            _client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
            _client.DefaultRequestHeaders.Add( "authorization", $"Bearer {token}" );

            var generator = new CryptoRandom();
            var password = generator.Next( 10, 1000000 ).ToString( "D10" );

            var parameters = new Dictionary<string, string>
            {
                {"email", email },
                {"name", name },
                {"username", name.Trim().Replace( " ", "_" ) },
                {"connection", "Username-Password-Authentication" },
                {"password", $"{password}!matri"}
            };

            var request = new HttpRequestMessage( HttpMethod.Post, $"https://{_authSettings.Domain}/api/v2/users" )
            {
                Content = new FormUrlEncodedContent( parameters )
            };

            var response = await _client.SendAsync( request );

            return (int) response.StatusCode;
        }
        catch ( Exception ex )
        {
            _logger.LogError( $"Error calling Create User" );
            throw new ServiceException( nameof( CreateUser ), "Error creating user", ex );
        }
    }
}
public record AuthResult
{
    [JsonPropertyName( "access_token" )]
    public string AccessToken { get; set; }
    public string Scope { get; set; }
    [JsonPropertyName( "expires_in" )]
    public int ExpiresIn { get; set; }
    [JsonPropertyName( "token_type" )]
    public string TokenType { get; set; }

}
