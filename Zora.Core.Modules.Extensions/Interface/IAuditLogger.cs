using Zora.Modules.Extensions.Helpers;

namespace Zora.Modules.Extensions.Interface
{
    public interface IAuditLogger
    {
        Task LogChangesAsync(List<ChangeLog> changes);

    }
}
