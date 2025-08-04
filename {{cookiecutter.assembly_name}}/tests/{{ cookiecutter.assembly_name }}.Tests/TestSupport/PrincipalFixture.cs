using System.Security.Claims;
using NSubstitute;
using {{cookiecutter.assembly_name}}.Core.Identity;

namespace {{cookiecutter.assembly_name}}.Tests.TestSupport;

public class PrincipalFixture
{
    public static IPrincipalProvider Next(
        string emailType = AuthConstants.Claim.Email,
        string email = null
    )
    {
        var principleProvider = Substitute.For<IPrincipalProvider>();
        principleProvider.User.Returns( new ClaimsPrincipal( new ClaimsIdentity[]
        {
            new(new List<Claim>
            {
                new(emailType, email ?? $"email-{Guid.NewGuid():N}")
            })
        } ) );

        return principleProvider;
    }
}
