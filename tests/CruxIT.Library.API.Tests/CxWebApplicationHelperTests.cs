using AutoMapper;
using CruxIT.Library.API;
using CruxIT.Library.API.DTO;
using CruxIT.Library.API.Tests.Fixtures;
using CruxIT.Library.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http.Json;

namespace CruxIT.Library.API.Tests;

public sealed class CxWebApplicationHelperTests
{
    [Fact]
    public async Task MiddlewareExtensions_InstallRequestAndExceptionLogging()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<IMapper>(_ => TestMapperFactory.Create());
        await using var app = builder.Build();
        app.LogOnExceptionHandler();
        app.LogOnRequest();
        app.Run(_ => Task.FromException(new CxException(CxHttpStatusCode.UnprocessableEntity, "Invalid request.")
        {
            MessageResourceKey = "InvalidRequest"
        }));

        await app.StartAsync();
        var response = await app.GetTestClient().GetAsync("/test");
        var body = await response.Content.ReadFromJsonAsync<CxExceptionResponse>();
        await app.StopAsync();

        Assert.Equal(422, (int)response.StatusCode);
        Assert.Equal("InvalidRequest", body!.MessageResourceKey);
        Assert.Equal("Invalid request.", body.Message);
    }
}
