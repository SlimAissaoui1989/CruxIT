using CruxIT.Library.Exceptions;

namespace CruxIT.Library.Tests.Exceptions;

public class CxExceptionTests
{
    [Fact]
    public void Constructor_WithStatusCodeAndMessage_StoresBoth()
    {
        var exception = new CxException(CxHttpStatusCode.NotFound, "Record was not found.");

        Assert.Equal(CxHttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Record was not found.", exception.Message);
    }
}
