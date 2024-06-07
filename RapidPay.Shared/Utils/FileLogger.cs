using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Shared.Utils
{
    public class FileLogger
    {
        private readonly string _logFilePath;
        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public async Task LogAsync(string message)
        {
            var logMessage = $"{DateTime.UtcNow}: {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(_logFilePath, logMessage);
        }
    }
}
