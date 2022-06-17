using System.IO;
using AutoFixture;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Modules.GlucoseData.Tests.Customizations;

public class FormFileCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IFormFile>(composer => composer.FromFactory
            (
                () =>
                {
                    MemoryStream contentStream = new(File.ReadAllBytes("Customizations/LibreViewData.csv"));
                    return new FormFile
                    (
                        contentStream, 
                        contentStream.Position,
                        contentStream.Length,
                        name:     "file", 
                        fileName: "file"
                    );
                })
        );
    }
}