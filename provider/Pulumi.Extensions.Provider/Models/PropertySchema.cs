using System.Text.Json.Serialization;

namespace Pulumi.Extensions.Provider.Models
{
    public class PropertySchema : BaseTypeSchema
    {
        public string? Description { get; set; }
        public string? DeprecationMessage { get; set; }
        public bool? ReplaceOnChanges { get; set; }
        public bool? WillReplaceOnChanges { get; set; }

        [JsonPropertyName("secret")]
        public bool? IsSecret { get; set; }

        [JsonIgnore]
        public bool IsRequired { get; set; }
    }
}
