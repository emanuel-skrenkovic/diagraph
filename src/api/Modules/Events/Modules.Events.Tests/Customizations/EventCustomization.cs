using System;
using System.Collections.Generic;
using AutoFixture;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class EventCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Event>(composer => composer.FromFactory
        (
            () => new()
            {
                Text          = fixture.Create<string>(),
                OccurredAtUtc = fixture.Create<DateTime>(),
                Tags          = fixture.Create<ICollection<EventTag>>()
            }).OmitAutoProperties()
        );
    }
}