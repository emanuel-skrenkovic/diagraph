using System;
using AutoFixture;

namespace Diagraph.Infrastructure.Tests.AutoFixture.Customizations;

public class DateTimeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<DateTime>(composer => composer.FromFactory(() => DateTime.UtcNow));
    }
}