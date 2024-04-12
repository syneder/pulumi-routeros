using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pulumi.Experimental.Provider;
using System.Diagnostics;

namespace Pulumi.Extensions.Provider
{
    using PropertiesDictionary = IDictionary<string, PropertyValue>;

    public abstract partial class ProviderContext : IDisposable
    {
        private readonly Dictionary<string, ResourceDescriptor> _resourceDescriptors = [];
        private ServiceProvider? _resourceInstanceFactory;

        public abstract string Schema { get; }

        public virtual Resource CreateResourceInstance(string resourceType, PropertiesDictionary properties)
        {
            if (!_resourceDescriptors.TryGetValue(resourceType, out var resourceDescriptor))
            {
                throw new InvalidOperationException($"The resource type {resourceType} is not registered.");
            }

            Debug.Assert(resourceDescriptor.InstanceType != null);
            var resourceInstance = CreateResourceInstance(resourceDescriptor.InstanceType);
            resourceInstance.Descriptor = resourceDescriptor;

            foreach (var propertiesSetter in resourceDescriptor.PropertiesSetters)
            {
                propertiesSetter.Invoke(resourceInstance, properties);
            }

            return resourceInstance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected ResourceDescriptor<T> RegisterResource<T>(string resourceType) where T : Resource
        {
            if (!_resourceDescriptors.TryGetValue(resourceType, out var descriptor))
            {
                _resourceDescriptors.Add(resourceType, descriptor = new ResourceDescriptor<T>(resourceType));
            }
            else
            {
                if (descriptor.InstanceType != typeof(T))
                {
                    throw new InvalidOperationException(
                        $"The resource type {resourceType} has already been added to the descriptor collection, " +
                        $"but the requested instance type {typeof(T).Name} is different from the instance type in the descriptor.");
                }

                var existedDescriptor = _resourceDescriptors.Values.FirstOrDefault(value => value.InstanceType == typeof(T));
                if (existedDescriptor != default)
                {
                    throw new InvalidOperationException(
                        $"Cannot add resource type {resourceType} for instance type {typeof(T).Name} " +
                        $"to the descriptor collection because another resource type {existedDescriptor.ResourceType} " +
                        $"has already been added to the collection for that instance type.");
                }
            }

            Debug.Assert(descriptor is ResourceDescriptor<T>);
            return (ResourceDescriptor<T>)descriptor;
        }

        protected virtual void BuildResourceInstanceFactories(IList<ServiceDescriptor> serviceDescriptors)
        {
            if (_resourceInstanceFactory == default)
            {
                var serviceCollection = serviceDescriptors.CreateServiceCollection();
                foreach (var (_, resourceDescriptor) in _resourceDescriptors)
                {
                    Debug.Assert(resourceDescriptor.InstanceType != null);
                    serviceCollection.TryAddTransient(resourceDescriptor.InstanceType);
                }

                _resourceInstanceFactory = serviceCollection.BuildServiceProvider();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _resourceInstanceFactory?.Dispose();
            }
        }

        private Resource CreateResourceInstance(Type instanceType)
        {
            if (_resourceInstanceFactory == default)
            {
                throw new InvalidOperationException(
                    $"A resource instance of type {instanceType.Name} cannot be created because " +
                    $"the resource instance factory was not created using the {nameof(BuildResourceInstanceFactories)} method.");
            }

            var resourceInstance = _resourceInstanceFactory.GetRequiredService(instanceType);
            Debug.Assert(resourceInstance is Resource);
            return (Resource)resourceInstance;
        }
    }
}
