using Pulumi.Experimental.Provider;

namespace Pulumi.Extensions.Provider
{
    public abstract class Provider(ProviderContext context) : Experimental.Provider.Provider
    {
        public override Task<ConfigureResponse> Configure(ConfigureRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ConfigureResponse { });
        }

        public override Task<CheckResponse> Check(CheckRequest request, CancellationToken ct)
        {
            return Task.FromResult(new CheckResponse
            {
                Failures = new List<CheckFailure>(),
                Inputs = request.NewInputs
            });
        }

        public override async Task<CreateResponse> Create(CreateRequest request, CancellationToken cancellationToken)
        {
            if (request.Timeout == TimeSpan.Zero)
            {
                return await CreateAsync(request, cancellationToken);
            }

            using var cancellationTokenSource = new CancellationTokenSource(request.Timeout);
            using (cancellationToken.Register(cancellationTokenSource.Cancel))
            {
                return await CreateAsync(request, cancellationTokenSource.Token);
            }
        }

        public override Task<ReadResponse> Read(ReadRequest request, CancellationToken cancellationToken)
        {
            return base.Read(request, cancellationToken);
        }

        public override Task<UpdateResponse> Update(UpdateRequest request, CancellationToken cancellationToken)
        {
            return base.Update(request, cancellationToken);
        }

        public override Task Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            return base.Delete(request, cancellationToken);
        }

        public override Task<GetSchemaResponse> GetSchema(GetSchemaRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new GetSchemaResponse { Schema = context.Schema });
        }

        private async Task<CreateResponse> CreateAsync(CreateRequest request, CancellationToken cancellationToken)
        {
            var resource = context.CreateResourceInstance(request.Type, request.Properties);
            resource.Id = await resource.CreateAsync(cancellationToken);

            return new CreateResponse
            {
                Id = resource.Id,
                Properties = resource.GetOutputs(),
            };
        }
    }
}
