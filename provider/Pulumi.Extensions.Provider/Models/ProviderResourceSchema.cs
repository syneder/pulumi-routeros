namespace Pulumi.Extensions.Provider.Models
{
    public class ProviderResourceSchema : TypeSchema
    {
        public string? DeprecationMessage { get; set; }

        public Dictionary<string, PropertySchema> InputProperties { get; set; } = [];
        public IEnumerable<string> RequiredInputs => InputProperties.Where(var => var.Value.IsRequired).Select(var => var.Key);
    }
}
