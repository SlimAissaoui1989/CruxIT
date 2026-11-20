using CruxIT.Library;

namespace CruxIT.Library.Tests;

public class CxMimeTypeHelperTests
{
    [Theory]
    [InlineData(".pdf", "application/pdf")]
    [InlineData("PDF", "application/pdf")]
    [InlineData(".unknown", "application/octet-stream")]
    public void GetContentType_ReturnsMimeTypeOrBinaryFallback(string extension, string expected)
    {
        Assert.Equal(expected, CxMimeTypeHelper.GetContentType(extension));
    }

    [Theory]
    [InlineData("image/png", true)]
    [InlineData("application/pdf", true)]
    [InlineData("video/mp4", false)]
    public void CanVisualizeInBrowser_ReportsSupportedTypes(string contentType, bool expected)
    {
        Assert.Equal(expected, CxMimeTypeHelper.CanVisualizeInBrowser(contentType));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetContentType_RejectsMissingExtension(string? extension)
    {
        Assert.Throws<ArgumentException>(() => CxMimeTypeHelper.GetContentType(extension!));
    }
}
