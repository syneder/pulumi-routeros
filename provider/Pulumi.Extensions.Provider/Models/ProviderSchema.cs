using System.Text.Json.Serialization;

namespace Pulumi.Extensions.Provider.Models
{
    public class ProviderSchema
    {
        public required string Name { get; set; }
        public string? DisplayName { get; set; }
        public required Version Version { get; set; }
        public string? Description { get; set; }
        public string? License { get; set; }
        public string? Attribution { get; set; }
        public string? Repository { get; set; }
        public string? Publisher { get; set; }
        public ProviderResourceSchema? Provider { get; set; }

        [JsonPropertyName("config")]
        public ProviderConfigurationSchema Configuration { get; set; } = new();

        public IEnumerable<string>? Keywords { get; set; }
        public Dictionary<string, ComplexTypeSchema> Types { get; set; } = [];
        public Dictionary<string, ProviderResourceSchema> Resources { get; set; } = [];
    }
}
