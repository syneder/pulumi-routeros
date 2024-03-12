using Microsoft.Extensions.DependencyInjection;
using Pulumi.Experimental.Provider;
using Pulumi.Extensions.Provider.Abstractions;
using Pulumi.Extensions.Provider.Exceptions;

namespace Pulumi.Extensions.Provider
{
    public abstract class ProviderContext
    {
        protected abstract IDictionary<string, ResourceSerializer> ResourceSerializers { get; }
        protected abstract IDictionary<string, Type> ResourceServiceFactories { get; }

        public abstract string Schema { get; }

        public ResourceBase DeserializeResource(string resourceType, string resourceName, IDictionary<string, PropertyValue> properties)
        {
            var resource = GetResourceSerializer(resourceType).Deserialize(properties);
            resource.Name = resourceName;
            return resource;
        }

        public IDictionary<string, PropertyValue> SerializeResource(string resourceType, ResourceBase resource)
        {
            return GetResourceSerializer(resourceType).Serialize(resource);
        }

        public ResourceServiceBase CreateResourceService(string resourceType, IServiceProvider serviceProvider)
        {
            if (!ResourceServiceFactories.TryGetValue(resourceType, out var resourceServiceType))
            {
                throw new ResourceTypeNotSupportedException(resourceType);
            }

            return serviceProvider.GetRequiredService(resourceServiceType) as ResourceServiceBase
                ?? throw new InvalidOperationException($"{resourceServiceType.Name} service are not inherited from {nameof(ResourceServiceBase)}");
        }

        private ResourceSerializer GetResourceSerializer(string resourceType)
        {
            if (!ResourceSerializers.TryGetValue(resourceType, out var resourceSerializer))
            {
                throw new ResourceTypeNotSupportedException(resourceType);
            }

            return resourceSerializer;
        }
    }
}
