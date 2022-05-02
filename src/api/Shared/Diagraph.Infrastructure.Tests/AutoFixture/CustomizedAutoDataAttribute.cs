using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;

namespace Diagraph.Infrastructure.Tests.AutoFixture;

public class CustomizedAutoDataAttribute : AutoDataAttribute
{
    private static IFixture _customizedFixture;
    
    public CustomizedAutoDataAttribute()
        : base(() => _customizedFixture) { }

    static CustomizedAutoDataAttribute()
        => CustomizeFixture();

    private static void CustomizeFixture()
    {
        _customizedFixture = new Fixture();

        foreach (ICustomization customization in GetCustomizations())
            _customizedFixture.Customize(customization);
    }

    private static IEnumerable<ICustomization> GetCustomizations()
    {
        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Type customizationType = typeof(ICustomization);
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => customizationType.IsAssignableFrom(t) && (!t.Namespace?.StartsWith("AutoFixture") ?? false))
            .Select(t => (ICustomization)Activator.CreateInstance(t));
    }
}
