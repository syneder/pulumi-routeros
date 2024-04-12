using Pulumi.Experimental.Provider;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Pulumi.Extensions.Provider
{
    using PropertiesDictionary = IDictionary<string, PropertyValue>;

    public abstract partial class ProviderContext
    {
        protected internal record ResourceDescriptor
        {
            private readonly IList<Action<Resource, PropertiesDictionary>> _propertiesSetters = [];
            private readonly IList<Func<Resource, PropertiesDictionary, PropertiesDictionary>> _outputsGetters = [];

            public ImmutableList<Action<Resource, PropertiesDictionary>> PropertiesSetters
            {
                get => _propertiesSetters.ToImmutableList();
            }

            public ImmutableList<Func<Resource, PropertiesDictionary, PropertiesDictionary>> OutputsGetters
            {
                get => _outputsGetters.ToImmutableList();
            }

            public string ResourceType { get; }
            public Type? InstanceType { get; protected set; }

            protected ResourceDescriptor(string resourceType) => ResourceType = resourceType;

            protected void AddPropertiesSetter(Action<Resource, PropertiesDictionary> propertiesSetter)
            {
                _propertiesSetters.Add(propertiesSetter);
            }

            protected void AddOutputsGetter(Func<Resource, PropertiesDictionary, PropertiesDictionary> outputsGetter)
            {
                _outputsGetters.Add(outputsGetter);
            }
        }

        protected internal record ResourceDescriptor<T> : ResourceDescriptor where T : Resource
        {
            public ResourceDescriptor(string resourceType) : base(resourceType)
            {
                InstanceType = typeof(T);
            }

            public ResourceDescriptor<T> AddPropertiesSetter(Action<T, PropertiesDictionary> propertiesSetter)
            {
                base.AddPropertiesSetter((instance, properties) =>
                {
                    Debug.Assert(instance is T);
                    propertiesSetter.Invoke((T)instance, properties);
                });

                return this;
            }

            public ResourceDescriptor<T> AddOutputsGetter(Func<T, PropertiesDictionary, PropertiesDictionary> outputsGetter)
            {
                base.AddOutputsGetter((instance, properties) =>
                {
                    Debug.Assert(instance is T);
                    return outputsGetter.Invoke((T)instance, properties);
                });

                return this;
            }
        }
    }
}
