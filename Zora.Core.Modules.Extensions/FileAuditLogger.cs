using System.Text.Json;
using Zora.Modules.Extensions.Helpers;
using Zora.Modules.Extensions.Interface;

namespace Zora.Modules.Extensions
{
    public class FileAuditLogger : IAuditLogger
    {
        private readonly string _logFilePath;

        public FileAuditLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public async Task LogChangesAsync(List<ChangeLog> changes)
        {
            var json = JsonSerializer.Serialize(changes, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.AppendAllTextAsync(_logFilePath, json);
        }
    }
}
