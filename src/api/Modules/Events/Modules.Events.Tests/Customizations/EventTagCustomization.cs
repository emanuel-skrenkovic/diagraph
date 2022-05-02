using AutoFixture;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class EventTagCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<EventTag>(composer => composer.FromFactory
        (
            () => new()
            {
                Name = fixture.Create<string>(),
            }).OmitAutoProperties()
        );
    }
}