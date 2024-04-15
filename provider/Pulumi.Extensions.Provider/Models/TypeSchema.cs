namespace Pulumi.Extensions.Provider.Models
{
    public class TypeSchema
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public Dictionary<string, PropertySchema> Properties { get; set; } = [];
        public IEnumerable<string> Required => Properties.Where(var => var.Value.IsRequired).Select(var => var.Key);
    }
}
