using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API.Helper
{
    public static class CxHttpContextHelper
    {
        public static IPAddress? GetIpAdress(HttpContext httpContext)
        {
            string? clientIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            IPAddress? result = null;

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (!string.IsNullOrEmpty(clientIp))
            {
                result = IPAddress.Parse(clientIp!);
                if (result.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    result = Dns.GetHostEntry(result)
                        .AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            return result;
        }
    }
}
