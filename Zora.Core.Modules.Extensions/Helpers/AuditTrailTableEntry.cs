using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zora.Modules.Extensions.Helpers
{
    public class AuditTrailTableEntry : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public long RecordId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }   
        public string Activity { get; set; }

        public ETag ETag { get; set; }
    }
}
