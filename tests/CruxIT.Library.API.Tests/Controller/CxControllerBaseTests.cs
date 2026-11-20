using CruxIT.Library.API;
using CruxIT.Library.API.Controller;
using CruxIT.Library.API.Tests.Fixtures;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Security.Claims;

namespace CruxIT.Library.API.Tests.Controller;

public sealed class CxControllerBaseTests
{
    [Fact]
    public void ControllerContext_ParsesRequestIdentityAndCopiesMetadataToUnitOfWork()
    {
        var userId = Guid.NewGuid();
        var remoteIp = IPAddress.Parse("192.0.2.30");
        var dbContext = new TestDbContext();
        var unitOfWork = new UnitOfWork<TestDbContext>(dbContext);
        var controllerLogger = NullLogger<CxControllerBase<TestDbContext, UnitOfWork<TestDbContext>>>.Instance;
        var controller = new TestController(
            new ConfigurationBuilder().Build(),
            controllerLogger,
            unitOfWork,
            TestMapperFactory.Create());
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(CxLibraryApiConstValues.UserIdTag, userId.ToString()),
                new Claim(CxLibraryApiConstValues.UserNameTag, "Ada"),
                new Claim(CxLibraryApiConstValues.RoleIdTag, "7"),
                new Claim(CxLibraryApiConstValues.RoleNameTag, "Administrator")
            ], "test"))
        };
        httpContext.Request.Headers.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        httpContext.Request.Headers[CxLibraryApiConstValues.LanguageTag] = "fr-TN";
        httpContext.Connection.RemoteIpAddress = remoteIp;

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        Assert.Equal(userId, controller.CurrentUserId);
        Assert.Equal("Ada", controller.CurrentUserName);
        Assert.Equal(7, controller.CurrentRoleId);
        Assert.Equal("Administrator", controller.CurrentRoleName);
        Assert.Equal("fr-TN", controller.CurrentLanguageId);
        Assert.Equal(remoteIp, controller.IpAdress);
        Assert.False(string.IsNullOrWhiteSpace(controller.Browser));
        Assert.False(string.IsNullOrWhiteSpace(controller.OS));
        Assert.Equal(userId, unitOfWork.CurrentUserId);
        Assert.Equal("Ada", unitOfWork.CurrentUserName);
        Assert.Equal(7, unitOfWork.CurrentRoleId);
        Assert.Equal("Administrator", unitOfWork.CurrentRoleName);
        Assert.Equal("fr-TN", unitOfWork.CurrentLanguageId);
        Assert.Equal(remoteIp.ToString(), unitOfWork.IpAdress);
        Assert.Equal(controller.Browser, unitOfWork.Browser);
        Assert.Equal(controller.OS, unitOfWork.OS);
        Assert.Equal(controller.Device, unitOfWork.Device);
        Assert.Same(controller.Mapper, unitOfWork.Mapper);

        unitOfWork.Dispose();
    }
}
