namespace Zora.Modules.Extensions.Helpers
{
    public class ChangeLog
    {
        public string? EntityName { get; set; }
        public object? PrimaryKeyValue { get; set; }
        public string? State { get; set; }
        public Dictionary<string, string>? OriginalValues { get; set; }
        public Dictionary<string, string>? CurrentValues { get; set; }
    }
}
