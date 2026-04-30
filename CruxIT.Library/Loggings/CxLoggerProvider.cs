using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.Loggings
{
    public class CxLoggerProvider : ILoggerProvider
    {
        private readonly string _basePath;

        public CxLoggerProvider(string basePath = null!)
        {
            _basePath = basePath ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CxLogger(categoryName, _basePath);
        }

        public void Dispose() { }
    }
}
