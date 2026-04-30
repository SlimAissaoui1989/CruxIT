using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.Loggings
{
    public class CxLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _basePath;
        private static readonly object _lock = new();

        public CxLogger(string categoryName, string basePath = null!)
        {
            _categoryName = categoryName;
            _basePath = basePath ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        }

        public IDisposable BeginScope<TState>(TState state) 
            where TState : notnull 
                => new LoggerScope();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            string message = formatter(state, exception);
            string errorPath = Path.Combine(_basePath, "Error");
            string targetPath = logLevel >= LogLevel.Error ? errorPath : _basePath;
            EnsureDirectory(targetPath);

            string prefix = logLevel >= LogLevel.Error ? "Err-" : "";
            string fileName = $"{prefix}Log_{DateTime.Today:yyyy-MM-dd}.txt";
            string filePath = Path.Combine(targetPath, fileName);

            string method = ExtractCallerMethod();
            string formattedLog =
$@"____________________________________________________________________________
[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]
---------------------
---> Category: {_categoryName}
---> Method: {method}
---------------------
---> Level: {logLevel}
---------------------
---> Message: {message}
{(exception != null ? $"Exception: {exception}" : "")}
";

            lock (_lock)
            {
                File.AppendAllText(filePath, formattedLog + Environment.NewLine);
            }

            // Optional: write to console
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {message}");
        }

        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private string ExtractCallerMethod()
        {
            try
            {
                string[] callStack = Environment.StackTrace.Split(" at ");
                // Adjust index depending on depth (4 works well for most)
                return callStack.Length > 4 ? callStack[4].Trim() : "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
