using Pulumi.Extensions.Provider.Abstractions;

namespace Pulumi.Extensions.Provider
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#pragma warning disable CS9113 // Parameter is unread
    public class ProviderResourceGenerationAttribute<T>(string name) : Attribute where T : ResourceServiceBase { }
#pragma warning restore CS9113 // Parameter is unread
}
