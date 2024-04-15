namespace Pulumi.Extensions.Provider.Models
{
    public class ComplexTypeSchema : TypeSchema
    {
        public IEnumerable<EnumValueSchema>? Enum { get; set; }
    }
}
