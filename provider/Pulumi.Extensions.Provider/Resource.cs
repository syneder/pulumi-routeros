using Pulumi.Experimental.Provider;

namespace Pulumi.Extensions.Provider
{
    public abstract class Resource
    {
        public string? Id { get; internal set; }
        internal ProviderContext.ResourceDescriptor? Descriptor { get; set; }

        public abstract Task<string> CreateAsync(CancellationToken cancellationToken);

        public virtual IDictionary<string, PropertyValue> GetOutputs()
        {
            if (Descriptor == default)
            {
                throw new InvalidOperationException(
                    $"The outputs cannot be obtained because the {nameof(Descriptor)} is not set.");
            }

            IDictionary<string, PropertyValue> outputs = new Dictionary<string, PropertyValue>();
            foreach (var outputsGetter in Descriptor.OutputsGetters)
            {
                outputs = outputsGetter.Invoke(this, outputs);
            }

            return outputs;
        }
    }
}
