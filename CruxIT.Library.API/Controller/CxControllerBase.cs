using AutoMapper;
using CruxIT.Library.API.Helper;
using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace CruxIT.Library.API.Controller
{
    public abstract class CxControllerBase<TDbContext, TUnitOfWork> : ControllerBase
        where TDbContext : CxDbContext, new()
        where TUnitOfWork : IUnitOfWork<TDbContext>
    {
        private bool _disposed;
        protected IConfiguration Config { get; }
        protected ILogger<CxControllerBase<TDbContext, TUnitOfWork>> Logger { get; }
        protected TUnitOfWork UnitOfWork { get; }

        public int? CurrentUserId { get; private set; }
        public string? CurrentUserName { get; private set; }
        public int? CurrentRoleId { get; private set; }
        public string? CurrentRoleName { get; private set; }

        public string? CurrentLanguageId { get; private set; }
        public virtual IPAddress? IpAdress { get; private set; }

        public virtual string? Browser { get; private set; }
        public virtual string? OS { get; private set; }
        public virtual string? Device { get; private set; }

        public IMapper Mapper { get; }

        [ControllerContext]
        public new ControllerContext ControllerContext
        {
            get => base.ControllerContext;
            set
            {
                base.ControllerContext = value;
                InitilizeRequestInfoValues();

                UnitOfWork.CurrentUserId = CurrentUserId;
                UnitOfWork.CurrentUserName = CurrentUserName;

                UnitOfWork.CurrentRoleId = CurrentRoleId;
                UnitOfWork.CurrentRoleName = CurrentRoleName;

                UnitOfWork.CurrentLanguageId = CurrentLanguageId;

                UnitOfWork.IpAdress = IpAdress?.ToString();
                UnitOfWork.Browser = Browser;
                UnitOfWork.OS = OS;
                UnitOfWork.Device = Device;

                UnitOfWork.Mapper = Mapper;
            }
        }

        private void InitilizeRequestInfoValues()
        {

            string userAgentStr = HttpContext.Request.Headers["User-Agent"].ToString();
            if (userAgentStr == null) return;

            ClientInfo? userAgentInfo = Parser.GetDefault().Parse(userAgentStr);

            Browser = $"{userAgentInfo?.UA.Family} {userAgentInfo?.UA.Major}.{userAgentInfo?.UA.Minor}";
            HttpContext.Items[CxLabraryApiConstValues.BrowserItemTag] = Browser;

            OS = $"{userAgentInfo?.OS.Family} {userAgentInfo?.OS.Major}.{userAgentInfo?.OS.Minor}";
            HttpContext.Items[CxLabraryApiConstValues.OsTag] = OS;

            Device = userAgentInfo?.Device.Family;
            HttpContext.Items[CxLabraryApiConstValues.DeviceTag] = Device;

            IpAdress = CxHttpContextHelper.GetIpAdress(HttpContext);
            HttpContext.Items[CxLabraryApiConstValues.IpAdressTag] = IpAdress;

            if (int.TryParse(HttpContext.User?.FindFirstValue(CxLabraryApiConstValues.UserIdTag), out int userId))
            {
                CurrentUserId = userId;
                HttpContext.Items[CxLabraryApiConstValues.UserIdTag] = CurrentUserId;
            }

            CurrentUserName = HttpContext.User?.FindFirstValue(CxLabraryApiConstValues.UserNameTag);
            HttpContext.Items[CxLabraryApiConstValues.UserNameTag] = CurrentUserName;

            if (int.TryParse(HttpContext.User?.FindFirstValue(CxLabraryApiConstValues.RoleIdTag), out int roleId))
            {
                CurrentRoleId = roleId;
                HttpContext.Items[CxLabraryApiConstValues.RoleIdTag] = roleId;
            }
            CurrentRoleName = HttpContext.User?.FindFirstValue(CxLabraryApiConstValues.RoleNameTag);
            HttpContext.Items[CxLabraryApiConstValues.RoleNameTag] = CurrentRoleName;

            CurrentLanguageId = HttpContext.Request.Headers[CxLabraryApiConstValues.LanguageTag].ToString();
            HttpContext.Items[CxLabraryApiConstValues.LanguageTag] = CurrentLanguageId;
        }

        protected CxControllerBase(IConfiguration configuration,
            ILogger<CxControllerBase<TDbContext, TUnitOfWork>> logger,
            TUnitOfWork unitOfWork,
            IMapper mapper)
        {
            Config = configuration;
            Logger = logger;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
    }
}
