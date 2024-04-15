namespace Pulumi.Extensions.Provider.Models
{
    public class EnumValueSchema
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public object? Value { get; set; }
        public string? DeprecationMessage { get; set; }
    }
}
