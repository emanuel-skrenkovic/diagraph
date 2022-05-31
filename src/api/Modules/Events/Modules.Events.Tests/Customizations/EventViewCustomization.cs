using System;
using System.Collections.Generic;
using AutoFixture;
using Diagraph.Modules.Events.Api.Events.Commands;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class EventViewCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<EventView>(composer => composer.FromFactory
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