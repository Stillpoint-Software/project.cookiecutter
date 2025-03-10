using System.Security.Claims;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;
using {{cookiecutter.assembly_name}}.Api.Identity;
using NSubstitute;

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
