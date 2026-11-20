using CruxIT.Library.API.DTO;
using CruxIT.Library.API.Middlewares;
using CruxIT.Library.API.Tests.Fixtures;
using CruxIT.Library.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text.Json;

namespace CruxIT.Library.API.Tests.Middlewares;

public sealed class ExceptionLoggingTests
{
    [Fact]
    public async Task InvokeAsync_MapsCustomExceptionToJsonResponse()
    {
        var exception = new CxException(CxHttpStatusCode.Conflict, "Duplicate entry.")
        {
            MessageResourceKey = "DuplicateEntry",
            MessageResourceArgs = ["item-8"]
        };
        var middleware = CreateMiddleware(_ => Task.FromException(exception), Environments.Production);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<CxExceptionResponse>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal("Duplicate entry.", response!.Message);
        Assert.Equal("DuplicateEntry", response.MessageResourceKey);
    }

    [Fact]
    public async Task InvokeAsync_UsesBadRequestWhenCustomExceptionHasNoStatusCode()
    {
        var middleware = CreateMiddleware(
            _ => Task.FromException(new CxException("Invalid request.")),
            Environments.Production);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Equal(0, context.Response.Body.Length);
    }

    [Fact]
    public async Task InvokeAsync_RethrowsUnexpectedExceptionInDevelopment()
    {
        var original = new InvalidOperationException("Unexpected failure.");
        var middleware = CreateMiddleware(_ => Task.FromException(original), Environments.Development);
        var context = CreateContext();

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.Same(original, thrown);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    private static ExceptionLogging CreateMiddleware(RequestDelegate next, string environmentName)
    {
        var environment = new TestWebHostEnvironment { EnvironmentName = environmentName };
        return new ExceptionLogging(
            next,
            NullLogger<ExceptionLogging>.Instance,
            environment,
            TestMapperFactory.Create());
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";
        context.Request.Body = new MemoryStream();
        return context;
    }
}
