using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Zora.Modules.Extensions.Helpers;
using Zora.Modules.Extensions.Interface;
using Zora.Modules.Extensions.AppSettings;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Zora.Modules.Extensions
{
    public class AuditTrailInterceptor : SaveChangesInterceptor
    {
        private readonly IAuditLogger _auditLogger;

        public AuditTrailInterceptor(IAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            var changes = GetChangedEntities(context);

            if (changes.Any())
            {
                await _auditLogger.LogChangesAsync(changes);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private List<ChangeLog> GetChangedEntities(DbContext context)
        {
            var changes = new List<ChangeLog>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                {
                    var changeLog = CreateChangeLog(entry);
                    if (changeLog != null)
                    {
                        changes.Add(changeLog);
                    }
                }
            }

            return changes;
        }

        private ChangeLog? CreateChangeLog(EntityEntry entry)
        {
            var originalValues = new Dictionary<string, string>();
            var currentValues = new Dictionary<string, string>();

            string? primaryKeyName = null;
            object? primaryKeyValue = null;
            var primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey != null )
            {
                primaryKeyName = primaryKey.Properties[0].Name;
                primaryKeyValue = entry.OriginalValues[primaryKeyName];
            }
            if(entry.State == EntityState.Added)
            {
                primaryKeyValue = 0;
            }
            switch (entry.State)
            {
                case EntityState.Modified:
                    PopulateModifiedValues(entry, originalValues, currentValues, primaryKeyName);
                    break;
                case EntityState.Added:
                    PopulateAddedValues(entry, currentValues, primaryKeyName);
                    break;              
            }

            if (originalValues.Count > 0 || currentValues.Count > 0)
            {
                return new ChangeLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    State = entry.State.ToString(),
                    PrimaryKeyValue = primaryKeyValue,
                    OriginalValues = originalValues,
                    CurrentValues = currentValues
                };
            }

            return null;
        }

        private static void PopulateModifiedValues(EntityEntry entry, Dictionary<string, string> originalValues, Dictionary<string, string> currentValues, string? primaryKeyName)
        {
            foreach (var property in entry.OriginalValues.Properties)
            {
                var original = entry.OriginalValues[property]?.ToString();
                var current = entry.CurrentValues[property]?.ToString();

                if (original != current || property.Name == ExtensionConstants.ModifiedBy)
                {
                    originalValues[property.Name] = original;
                    currentValues[property.Name] = current;
                }
            }
        }

        private static void PopulateAddedValues(EntityEntry entry, Dictionary<string, string> currentValues, string? primaryKeyName)
        {
            var primaryKey = entry.Metadata.FindPrimaryKey();
            if (primaryKey != null) {
                primaryKeyName = primaryKey.Properties[0].Name;            }

            foreach (var property in entry.CurrentValues.Properties)
            {
                currentValues[property.Name] = property.Name == primaryKeyName ? "0" : (entry.CurrentValues[property]?.ToString() ?? string.Empty);
            }
        }
    }
}
