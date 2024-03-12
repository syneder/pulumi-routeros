namespace Pulumi.Extensions.Provider.Abstractions
{
    public abstract class ResourceServiceBase
    {
        public async Task CreateAsync(ResourceBase resource, bool isPreview, CancellationToken cancellationToken)
        {
            resource.Id = await CreateResourceAsync(resource, isPreview, cancellationToken);
        }

        public async Task<ResourceBase> ReadAsync(string resourceId, CancellationToken cancellationToken)
        {
            var resource = await ReadResourceAsync(resourceId, cancellationToken);
            resource.Id = resourceId;
            return resource;
        }

        public virtual Task DeleteAsync(string resourceId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException($"The method {nameof(DeleteAsync)} in {GetType().Name} is not implemented.");
        }

        protected virtual Task<string?> CreateResourceAsync(ResourceBase resource, bool isPreview, CancellationToken cancellationToken)
        {
            throw new NotImplementedException($"The method {nameof(CreateResourceAsync)} in {GetType().Name} is not implemented.");
        }

        protected virtual Task<ResourceBase> ReadResourceAsync(string resourceId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException($"The method {nameof(ReadResourceAsync)} in {GetType().Name} is not implemented.");
        }
    }

    public abstract class ResourceServiceBase<T> : ResourceServiceBase where T : ResourceBase
    {
        public virtual Task<IDictionary<string, T>> EnumerateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException($"The method {nameof(EnumerateAsync)} in {GetType().Name} is not implemented.");
        }
    }
}
