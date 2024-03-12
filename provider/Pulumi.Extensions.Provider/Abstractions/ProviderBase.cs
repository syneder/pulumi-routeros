using Microsoft.Extensions.DependencyInjection;
using Pulumi.Experimental.Provider;

namespace Pulumi.Extensions.Provider.Abstractions
{
    public abstract class ProviderBase : Experimental.Provider.Provider, IDisposable
    {
        private readonly ProviderContext _context;
        private readonly IServiceScope _serviceScope;

        protected ProviderBase(ProviderContext context, ServiceProvider serviceProvider)
        {
            _context = context;
            _serviceScope = serviceProvider.CreateScope();
        }

        public override Task<GetSchemaResponse> GetSchema(GetSchemaRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetSchemaResponse { Schema = _context.Schema });
        }

        public override async Task<CreateResponse> Create(CreateRequest request, CancellationToken cancellationToken)
        {
            using var cancellationTokenSource = new CancellationTokenSource(request.Timeout);
            using var cancellationTokenRegistration = cancellationToken.Register(cancellationTokenSource.Cancel);

            var resourceService = _context.CreateResourceService(request.Type, _serviceScope.ServiceProvider);
            var resource = _context.DeserializeResource(request.Type, request.Name, request.Properties);
            await resourceService.CreateAsync(resource, request.Preview, cancellationTokenSource.Token);

            return new CreateResponse
            {
                Id = resource.Id,
                Properties = _context.SerializeResource(request.Type, resource)
            };
        }

        public override async Task<ReadResponse> Read(ReadRequest request, CancellationToken cancellationToken)
        {
            var resourceService = _context.CreateResourceService(request.Type, _serviceScope.ServiceProvider);
            var resource = await resourceService.ReadAsync(request.Id, cancellationToken);

            return new ReadResponse
            {
                Id = resource.Id,
                Properties = _context.SerializeResource(request.Type, resource)
            };
        }

        public override Task Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            using var cancellationTokenSource = new CancellationTokenSource(request.Timeout);
            using var cancellationTokenRegistration = cancellationToken.Register(cancellationTokenSource.Cancel);

            var resourceService = _context.CreateResourceService(request.Type, _serviceScope.ServiceProvider);
            return resourceService.DeleteAsync(request.Id, cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serviceScope?.Dispose();
            }
        }
    }
}
