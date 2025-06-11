using Microsoft.EntityFrameworkCore;
using Zora.Modules.Extensions.Helpers;
using Zora.Modules.Extensions.Interface;

namespace Zora.Modules.Extensions
{
    public class DatabaseAuditLogger : IAuditLogger
    {
        private readonly DbContext _dbContext;

        public DatabaseAuditLogger(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogChangesAsync(List<ChangeLog> changes)
        {
            foreach (var change in changes)
            {
                //
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
