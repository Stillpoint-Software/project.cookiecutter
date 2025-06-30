using System.Security.Claims;

namespace {{cookiecutter.assembly_name}}.Api.Identity;

public interface IPrincipalProvider
{
    ClaimsPrincipal User { get; }
    public ClaimsIdentity Identity( string identityName );
    public string ClaimValue( string identityName, string claimType );
}
