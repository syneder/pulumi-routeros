using Microsoft.Extensions.DependencyInjection;
using Pulumi.Experimental.Provider;
using System.CommandLine.Invocation;

namespace Pulumi.RouterOS.Commands
{
    internal class ProviderCommandHandler(IServiceCollection services) : ICommandHandler
    {
        private readonly IServiceCollection _services = services ?? throw new ArgumentNullException(nameof(services));

        public int Invoke(InvocationContext context)
        {
            throw new NotSupportedException("Use InvokeAsync instead of Invoke");
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var args = context.ParseResult.Tokens.Select(token => token.Value);
            await InvokeAsync(args.ToArray(), context.GetCancellationToken());
            return default;
        }

        private Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            return Provider.Serve(args, null, providerHost =>
            {
                var serviceProvider = Startup.ConfigureProviderServices(_services, providerHost).BuildServiceProvider();
                return serviceProvider.GetRequiredService<Provider>();
            }, cancellationToken);
        }
    }
}
