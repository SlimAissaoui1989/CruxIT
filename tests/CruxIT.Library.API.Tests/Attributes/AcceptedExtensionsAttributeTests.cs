using CruxIT.Library.API.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CruxIT.Library.API.Tests.Attributes;

public sealed class AcceptedExtensionsAttributeTests
{
    [Fact]
    public void IsValid_AcceptsAllowedExtensionRegardlessOfFilenameCase()
    {
        var attribute = new AcceptedExtensionsAttribute([".jpg", ".png"]);
        using var stream = new MemoryStream();
        var file = new FormFile(stream, 0, 0, "file", "photo.JPG");

        var result = attribute.GetValidationResult(file, new ValidationContext(new object()));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_RejectsExtensionOutsideAllowList()
    {
        var attribute = new AcceptedExtensionsAttribute([".jpg", ".png"]);
        using var stream = new MemoryStream();
        var file = new FormFile(stream, 0, 0, "file", "document.pdf");

        var result = attribute.GetValidationResult(file, new ValidationContext(new object()));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains(".jpg, .png", result!.ErrorMessage);
    }

    [Fact]
    public void IsValid_AllowsNullSoRequiredValidationCanBeHandledSeparately()
    {
        var attribute = new AcceptedExtensionsAttribute([".jpg"]);

        var result = attribute.GetValidationResult(null, new ValidationContext(new object()));

        Assert.Equal(ValidationResult.Success, result);
    }
}
