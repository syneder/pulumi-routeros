using System.Text.Json.Serialization;

namespace Pulumi.Extensions.Provider.Models
{
    public class BaseTypeSchema
    {
        public string? Type { get; set; }

        [JsonPropertyName("items")]
        public BaseTypeSchema? ItemsType { get; set; }

        [JsonPropertyName("$ref")]
        public string? Reference { get; set; }
    }
}
