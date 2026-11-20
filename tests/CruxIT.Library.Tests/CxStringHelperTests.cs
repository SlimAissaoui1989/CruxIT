using CruxIT.Library;

namespace CruxIT.Library.Tests;

public class CxStringHelperTests
{
    [Fact]
    public void ComputeSha256Hash_ReturnsLowercaseSha256Hex()
    {
        var hash = CxStringHelper.ComputeSha256Hash("abc");

        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }

    [Theory]
    [InlineData("person@example.com", true)]
    [InlineData("person.name+tag@example.co.uk", true)]
    [InlineData("not-an-email", false)]
    [InlineData("person @example.com", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void IsEmail_ValidatesCommonEmailInputs(string value, bool expected)
    {
        Assert.Equal(expected, CxStringHelper.IsEmail(value));
    }

    [Theory]
    [InlineData("+1 555-123-4567", true)]
    [InlineData("+216 12 345 678", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void IsPhoneNumber_HandlesCommonInputs(string value, bool expected)
    {
        Assert.Equal(expected, CxStringHelper.IsPhoneNumber(value));
    }
}
