namespace Pulumi.Extensions.Provider.Models
{
    public class ProviderConfigurationSchema
    {
        public Dictionary<string, PropertySchema> Variables { get; set; } = [];
        public IEnumerable<string> Required => Variables.Where(var => var.Value.IsRequired).Select(var => var.Key);
    }
}
