using AutoMapper;
using CruxIT.Library.API.DTO;
using CruxIT.Library.API.Profiles;
using CruxIT.Library.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace CruxIT.Library.API.Tests.Profiles;

public sealed class CxExceptionProfileTests
{
    [Fact]
    public void MapsExceptionMessageAndResourceFieldsToResponse()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<CxExceptionProfile>(),
            NullLoggerFactory.Instance);
        configuration.AssertConfigurationIsValid();
        var exception = new CxException(CxHttpStatusCode.Conflict, "Duplicate record.")
        {
            MessageResourceKey = "DuplicateRecord",
            MessageResourceArgs = ["record-42"]
        };

        var response = configuration.CreateMapper().Map<CxExceptionResponse>(exception);

        Assert.Equal("Duplicate record.", response.Message);
        Assert.Equal("DuplicateRecord", response.MessageResourceKey);
        Assert.Equal("record-42", Assert.Single(response.MessageResourceArgs!));
    }
}
