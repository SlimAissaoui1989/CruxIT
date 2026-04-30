using AutoMapper;
using CruxIT.Library.API.DTO;
using CruxIT.Library.API.Profiles;
using CruxIT.Library.Exceptions;
using CruxIT.Library.Loggings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API.Middlewares
{
    public class ExceptionLogging
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLogging> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionLogging(RequestDelegate next, ILogger<ExceptionLogging> logger, IWebHostEnvironment env, IMapper mapper)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Enable request body buffering to allow seeking and rewinding
                context.Request.EnableBuffering();

                await _next(context); // Continue processing the request
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment() && ex is not CxException)
                    throw;

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception? ex)
        {
            // Check if exception is a custom CxException
            if (ex is CxException cxEx)
            {
                context.Response.StatusCode = (int?)cxEx.ExceptionType ?? (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                if (!string.IsNullOrEmpty(cxEx.Message)
                    && !string.IsNullOrEmpty(cxEx.MessageResourceKey))
                {
                    CxExceptionResponse response = new MapperConfiguration(cfg => cfg.AddProfile<CxExceptionProfile>(), NullLoggerFactory.Instance).CreateMapper().Map<CxExceptionResponse>(cxEx);
                    await context.Response.WriteAsJsonAsync(response);
                }
                return;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/plain";  // Set content type to plain text
                await context.Response.WriteAsync("An error occurred on the server");  // Write the plain text
            }


            // Log exception details
            var className = ex?.TargetSite?.DeclaringType?.Name ?? "Unknown";
            var logger = new CxLogger(className);

            var responseBuilder = new StringBuilder();
            string requestBody = string.Empty;

            // Log request body if it's available
            if (context.Request.Body.CanRead)
            {
                using (var stream = new MemoryStream())
                {
                    // make sure that body is read from the beginning
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await context.Request.Body.CopyToAsync(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());

                    // Reset the request body for the next middleware
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }

            // Log request method and path
            logger.LogCritical($"{context.Request.Method} {context.Request.Path} \n\r \n\r {requestBody}");

            // Log the exception details
            while (ex != null)
            {
                logger.LogError(ex.Message);
                ex = ex.InnerException;
            }
        }
    }
}
