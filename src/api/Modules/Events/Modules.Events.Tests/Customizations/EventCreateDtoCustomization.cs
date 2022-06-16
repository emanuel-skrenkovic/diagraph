using System;
using System.Collections.Generic;
using AutoFixture;
using Diagraph.Modules.Events.Api.Contracts;
using Diagraph.Modules.Events.Api.Events.Contracts;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class EventCreateDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<EventCreateDto>(composer => composer.FromFactory
        (
            () => new()
            {
                Text          = fixture.Create<string>(),
                OccurredAtUtc = fixture.Create<DateTime>(),
                Tags          = fixture.Create<ICollection<EventTagDto>>()
            }).OmitAutoProperties()
        );
    }
}