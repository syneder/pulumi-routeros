using Pulumi.Extensions.Provider.Abstractions;

namespace Pulumi.Extensions.Provider
{
    [AttributeUsage(AttributeTargets.Class)]
#pragma warning disable CS9113 // Parameter is unread
    public class ProviderGenerationAttribute<T>(string name) : Attribute where T : ProviderBase { }
#pragma warning restore CS9113 // Parameter is unread
}
