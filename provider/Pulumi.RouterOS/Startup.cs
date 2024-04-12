using Microsoft.Extensions.DependencyInjection;
using Pulumi.Experimental.Provider;
using Pulumi.RouterOS.Commands;
using System.CommandLine.Invocation;

namespace Pulumi.RouterOS
{
    internal class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICommandHandler>(serviceProvider =>
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                foreach (var serviceDescriptor in services)
                {
                    serviceCollection.Add(serviceDescriptor);
                }

                return new ProviderCommandHandler(serviceCollection);
            });

            services.AddScoped<ISchemaProviderCommandHandler, SchemaProviderCommandHandler>();
            return services;
        }

        public static IServiceCollection ConfigureProviderServices(IServiceCollection services, IHost providerHost)
        {
            services.AddScoped<Provider, Core.Provider>();
            services.AddScoped<Extensions.Provider.ProviderContext, Core.ProviderContext>();
            return services;
        }
    }
}
