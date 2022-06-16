using AutoFixture;
using Diagraph.Modules.Events.Api.Contracts;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class EventTagViewCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<EventTagDto>(composer => composer.FromFactory
        (
            () => new() { Name = fixture.Create<string>() }).OmitAutoProperties()
        );
    }
}