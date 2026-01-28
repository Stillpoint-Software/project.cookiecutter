
public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationHttpMessageHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var authenticateResult = await httpContext.AuthenticateAsync();

        return authenticateResult.Succeeded
            ? authenticateResult.Properties.GetTokenValue("access_token")
            : null;
    }
}