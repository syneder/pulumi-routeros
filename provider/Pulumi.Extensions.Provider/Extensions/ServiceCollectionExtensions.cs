namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection CreateServiceCollection(this IList<ServiceDescriptor> serviceDescriptors)
        {
            IServiceCollection services = new ServiceCollection();
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Add(serviceDescriptor);
            }

            return services;
        }
    }
}
