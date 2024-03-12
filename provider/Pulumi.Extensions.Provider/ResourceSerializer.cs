using Pulumi.Experimental.Provider;
using Pulumi.Extensions.Provider.Abstractions;

namespace Pulumi.Extensions.Provider
{
    public class ResourceSerializer
    {
        public required Func<IDictionary<string, PropertyValue>, ResourceBase> Deserialize { get; init; }
        public required Func<ResourceBase, IDictionary<string, PropertyValue>> Serialize { get; init; }
    }
}
