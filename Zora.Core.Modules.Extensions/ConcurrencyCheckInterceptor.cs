using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Zora.Modules.Extensions.AppSettings;

namespace Zora.Modules.Extensions
{
    public class ConcurrencyCheckInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified || x.State == EntityState.Deleted))
            {
                var databaseValues = entry.GetDatabaseValues();
                var databaseModifiedDateTime = (DateTime?)databaseValues?[ExtensionConstants.ModifiedDate];
                var currentModifiedDateTime = (DateTime?)entry.Property(ExtensionConstants.ModifiedDate).CurrentValue;
                if (databaseModifiedDateTime != null)
                {
                    // Compare ModifiedDateTime for concurrency check
                    if (databaseModifiedDateTime > currentModifiedDateTime)
                    {
                        var entityType = entry.Entity.GetType().Name;
                        throw new DbUpdateConcurrencyException($"Concurrency conflict detected for entity type '{entityType}'.");
                    }
                }
                entry.Property(ExtensionConstants.ModifiedDate).CurrentValue = DateTime.UtcNow;
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
