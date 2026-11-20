using CruxIT.Library.API;
using CruxIT.Library.API.Middlewares;
using CruxIT.Library.API.Tests.Fixtures;
using Microsoft.AspNetCore.Http;

namespace CruxIT.Library.API.Tests.Middlewares;

public sealed class RequestLoggingTests
{
    [Fact]
    public async Task InvokeAsync_CallsNextAndLogsRequestContext()
    {
        var nextCalled = false;
        var logger = new CapturingLogger<RequestLogging>();
        var middleware = new RequestLogging(context =>
        {
            nextCalled = true;
            context.Response.StatusCode = StatusCodes.Status201Created;
            return Task.CompletedTask;
        }, logger);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/items";
        context.Request.Headers.UserAgent = "test-agent";
        context.Items[CxLibraryApiConstValues.UserNameTag] = "Ada";

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        var message = Assert.Single(logger.Messages);
        Assert.Contains("Method: POST", message);
        Assert.Contains("Path: /api/items", message);
        Assert.Contains("UserName: Ada", message);
        Assert.Contains("Status: Created", message);
    }
}
