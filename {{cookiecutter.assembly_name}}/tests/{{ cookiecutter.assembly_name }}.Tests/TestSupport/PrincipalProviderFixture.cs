using System.Security.Claims;
using {{cookiecutter.assembly_name}}.Core.Identity;
using NSubstitute;

namespace {{cookiecutter.assembly_name}}.Tests.TestSupport;

public static class PrincipalProviderFixture
{
    public static IPrincipalProvider Next( ClaimsPrincipal principal = null )
    {
        var principalProvider = Substitute.For<IPrincipalProvider>();
        principalProvider.User.Returns( principal ?? new ClaimsPrincipal(
            new ClaimsIdentity( new[]
            {
                new Claim( AuthConstants.Claim.Email, "user@user.com" )
            } ) ) );

        return principalProvider;
    }
}
