using CruxIT.Library.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API
{
    public static class CxWebApplicationHelper
    {
        public static void LogOnExceptionHandler(this WebApplication app)
            => app.UseMiddleware<ExceptionLogging>();

        public static void LogOnRequest(this WebApplication app)
            => app.UseMiddleware<RequestLogging>();
    }
}
