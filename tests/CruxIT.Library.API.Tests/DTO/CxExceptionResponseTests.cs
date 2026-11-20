using CruxIT.Library.API.DTO;
using System.Text.Json;

namespace CruxIT.Library.API.Tests.DTO;

public sealed class CxExceptionResponseTests
{
    [Fact]
    public void SerializesAndDeserializesResponseFields()
    {
        var response = new CxExceptionResponse
        {
            Message = "Invalid input.",
            MessageResourceKey = "InvalidInput",
            MessageResourceArgs = ["name"]
        };

        var json = JsonSerializer.Serialize(response);
        var restored = JsonSerializer.Deserialize<CxExceptionResponse>(json);

        Assert.NotNull(restored);
        Assert.Equal(response.Message, restored.Message);
        Assert.Equal(response.MessageResourceKey, restored.MessageResourceKey);
        var argument = Assert.IsType<JsonElement>(Assert.Single(restored.MessageResourceArgs!));
        Assert.Equal("name", argument.GetString());
    }
}
