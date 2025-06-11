using System.Text.Json;
using Zora.Modules.Extensions.Helpers;
using Zora.Modules.Extensions.Interface;
using Azure.Data.Tables;
using System.Collections.Concurrent;

namespace Zora.Modules.Extensions
{
    public class AzureTableAuditLogger : IAuditLogger
    {
        private readonly TableClient tableClient;
        public AzureTableAuditLogger(TableClient tableClient)
        {
            this.tableClient = tableClient;
            tableClient.CreateIfNotExistsAsync();
        }

        public async Task LogChangesAsync(List<ChangeLog> changes)
        {  
            foreach (var change in changes)
            {
                var auditTrailTableEntry = new AuditTrailTableEntry
                {
                    PartitionKey = change.EntityName,
                    RowKey = Guid.NewGuid().ToString(),
                    RecordId= (change.PrimaryKeyValue as long?) ?? 0,
                    Activity = change.State,
                    OldValues = JsonSerializer.Serialize(change.OriginalValues),
                    NewValues = JsonSerializer.Serialize(change.CurrentValues)
                };

                // Log changes to Azure Table Storage
                await tableClient.UpsertEntityAsync(auditTrailTableEntry)
                   .ContinueWith(task =>
                   {
                       if (task.IsFaulted && task.Exception != null)
                       {
                           throw new InvalidOperationException("Upsert failed due to a network issue.", task.Exception.InnerException);
                       }
                   });
            }
        }
    }
}
