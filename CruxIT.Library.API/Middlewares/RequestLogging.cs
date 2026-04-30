using CruxIT.Library.API.Controller;
using CruxIT.Library.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace CruxIT.Library.API.Middlewares
{
    public class RequestLogging
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogging> _logger;

        public RequestLogging(RequestDelegate next, ILogger<RequestLogging> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Call the next middleware in the pipeline
            await _next(context);

            LogRequest(context);
        }

        private void LogRequest(HttpContext context)
        {
            // Capture request information
            var method = context.Request.Method;
            var path = context.Request.Path;

            string userAgentStr = context.Request.Headers["User-Agent"].ToString();
            if (userAgentStr == null) return;

            var browser = context.Items[CxLabraryApiConstValues.BrowserItemTag];

            var os = context.Items[CxLabraryApiConstValues.OsTag];

            var device = context.Items[CxLabraryApiConstValues.DeviceTag];

            var ipAddress = context.Items[CxLabraryApiConstValues.IpAdressTag];

            var currentUserId = context.Items[CxLabraryApiConstValues.UserIdTag];

            var currentUserName = context.Items[CxLabraryApiConstValues.UserNameTag];

            var currentRoleId = context.Items[CxLabraryApiConstValues.RoleIdTag];

            var currentRoleName = context.Items[CxLabraryApiConstValues.RoleNameTag];

            var currentLanguage = context.Items[CxLabraryApiConstValues.LanguageTag];

            string responseStatus = ((HttpStatusCode)context.Response.StatusCode).ToString();

            // Log the information
            _logger.LogInformation($"Request Info: Method: {method}" +
                $", Path: {path}" +
                $", Browser: {browser}" +
                $", OS: {os}" +
                $", Device: {device}" +
                $", IP: {ipAddress?.ToString()}" +
                $", UserId: {currentUserId}" +
                $", UserName: {currentUserName}" +
                $", RoleId: {currentRoleId}" +
                $", RoleName: {currentRoleName}" +
                $", Language: {currentLanguage}" +
                $", Status: {responseStatus}");
        }
    }
}
