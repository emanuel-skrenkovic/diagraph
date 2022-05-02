using AutoFixture;
using Diagraph.Modules.Identity.Api.Auth.Commands;

namespace Diagraph.Modules.Identity.Tests.Customizations;

public class UserRegisterCommandCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<UserRegisterCommand>
        (
            composer => composer.FromFactory
            (
                () => new ()
                {
                    Email = $"{fixture.Create<string>()}@domain.com",
                    Password = fixture.Create<string>()
                }
            ).OmitAutoProperties()
        );
    }
}