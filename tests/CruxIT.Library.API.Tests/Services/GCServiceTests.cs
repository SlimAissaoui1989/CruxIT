using CruxIT.Library.API.Services;

namespace CruxIT.Library.API.Tests.Services;

public sealed class GCServiceTests
{
    [Fact]
    public async Task DisposeAsync_CompletesSuccessfully()
    {
        var service = new GCService();

        await service.DisposeAsync();
    }
}
