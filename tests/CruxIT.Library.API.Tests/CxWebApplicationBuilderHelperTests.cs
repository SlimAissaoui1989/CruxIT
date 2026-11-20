using CruxIT.Library.API;
using CruxIT.Library.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;

namespace CruxIT.Library.API.Tests;

public sealed class CxWebApplicationBuilderHelperTests
{
    private const string JwtKey = "a-test-jwt-key-long-enough-to-use-with-hmac-sha256";

    [Fact]
    public void AddCorpsDefaultPolicyAllowAny_ConfiguresOpenCorsPolicy()
    {
        var builder = CreateBuilder();
        builder.AddCorpsDefaultPolicyAllowAny();
        using var provider = builder.Services.BuildServiceProvider();

        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
        var policy = corsOptions.GetPolicy(corsOptions.DefaultPolicyName)!;

        Assert.True(policy.AllowAnyOrigin);
        Assert.True(policy.AllowAnyHeader);
        Assert.True(policy.AllowAnyMethod);
    }

    [Fact]
    public void IgnoreCyclesJsonAndSavePropertyNamingPolicy_ConfiguresJsonOptions()
    {
        var builder = CreateBuilder();
        builder.IgnoreCyclesJsonAndSavePropertyNamingPolicy();
        using var provider = builder.Services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

        Assert.Equal(System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles, options.ReferenceHandler);
        Assert.Null(options.PropertyNamingPolicy);
    }

    [Fact]
    public async Task AddJWTAuthentication_ConfiguresValidationAndCookieToken()
    {
        var builder = CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Key"] = JwtKey
        });
        builder.AddJWTAuthentication("access_token");
        using var provider = builder.Services.BuildServiceProvider();

        var scheme = await provider.GetRequiredService<IAuthenticationSchemeProvider>().GetDefaultAuthenticateSchemeAsync();
        Assert.NotNull(scheme);
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(scheme.Name);
        Assert.Equal("test-issuer", options.TokenValidationParameters.ValidIssuer);
        Assert.Equal("test-issuer", options.TokenValidationParameters.ValidAudience);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "access_token=cookie-jwt";
        var messageContext = new MessageReceivedContext(httpContext, scheme, options);
        await options.Events.OnMessageReceived(messageContext);

        Assert.Equal("cookie-jwt", messageContext.Token);
    }

    [Fact]
    public async Task AddJWTAuthentication_WithoutCookieLeavesTokenUnset()
    {
        var builder = CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Key"] = JwtKey
        });
        builder.AddJWTAuthentication("access_token");
        using var provider = builder.Services.BuildServiceProvider();
        var scheme = await provider.GetRequiredService<IAuthenticationSchemeProvider>().GetDefaultAuthenticateSchemeAsync();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(scheme!.Name);
        var messageContext = new MessageReceivedContext(new DefaultHttpContext(), scheme, options);

        await options.Events.OnMessageReceived(messageContext);

        Assert.Null(messageContext.Token);
    }

    [Fact]
    public void AddJWTAuthentication_WithoutConfigurationDoesNotRegisterBearerScheme()
    {
        var builder = CreateBuilder();
        builder.AddJWTAuthentication();

        Assert.DoesNotContain(builder.Services, descriptor => descriptor.ServiceType == typeof(IAuthenticationSchemeProvider));
    }

    [Fact]
    public void AddSwaggerGenWithBearer_AddsBearerSecurityDefinitionAndRequirement()
    {
        var builder = CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwaggerGenWithBearer();
        using var provider = builder.Services.BuildServiceProvider();

        var document = provider.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");

        Assert.Contains("Bearer", document.Components!.SecuritySchemes!.Keys);
        Assert.NotEmpty(document.Security!);
    }

    [Fact]
    public async Task UseGC_RegistersScopedGcService()
    {
        var builder = CreateBuilder();
        builder.UseGC();
        await using var provider = builder.Services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();

        Assert.IsType<GCService>(scope.ServiceProvider.GetRequiredService<GCService>());
    }

    private static WebApplicationBuilder CreateBuilder()
    {
        return WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = [],
            EnvironmentName = Environments.Development
        });
    }
}
