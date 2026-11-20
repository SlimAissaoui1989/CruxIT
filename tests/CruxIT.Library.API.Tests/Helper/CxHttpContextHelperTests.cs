using CruxIT.Library.API.Helper;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace CruxIT.Library.API.Tests.Helper;

public sealed class CxHttpContextHelperTests
{
    [Fact]
    public void GetIpAdress_PrefersForwardedForHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "198.51.100.7";
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.4");

        Assert.Equal(IPAddress.Parse("198.51.100.7"), CxHttpContextHelper.GetIpAdress(context));
    }

    [Fact]
    public void GetIpAdress_FallsBackToRemoteAddress()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.4");

        Assert.Equal(IPAddress.Parse("192.0.2.4"), CxHttpContextHelper.GetIpAdress(context));
    }

    [Fact]
    public void GetIpAdress_ReturnsNullWhenNoAddressIsAvailable()
    {
        var context = new DefaultHttpContext();

        Assert.Null(CxHttpContextHelper.GetIpAdress(context));
    }

    [Fact]
    public void GetIpAdress_RejectsInvalidForwardedAddress()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "not-an-ip-address";

        Assert.Throws<FormatException>(() => CxHttpContextHelper.GetIpAdress(context));
    }
}
