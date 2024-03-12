namespace Pulumi.Extensions.Provider.Abstractions
{
    public abstract class ResourceBase
    {
        public string? Id { get; internal set; }
        public string? Name { get; internal set; }
    }
}
