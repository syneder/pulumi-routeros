using Pulumi.Experimental.Provider;
using Pulumi.Extensions.Provider.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pulumi.Extensions.Provider
{
    public abstract partial class Provider(ProviderContext context) : Experimental.Provider.Provider
    {
        public override Task<ConfigureResponse> Configure(ConfigureRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ConfigureResponse { });
        }

        public override Task<CheckResponse> Check(CheckRequest request, CancellationToken cancellationToken)
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

        public override async Task<GetSchemaResponse> GetSchema(GetSchemaRequest request, CancellationToken cancellationToken)
        {
            return new GetSchemaResponse
            {
                Schema = await SerializeSchemaAsync(context.Schema, cancellationToken)
            };
        }

        protected virtual async Task<string> SerializeSchemaAsync(ProviderSchema schema, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, schema, GetSchemaSerializerOptions(), cancellationToken);

            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            return await streamReader.ReadToEndAsync(cancellationToken);
        }

        protected virtual JsonSerializerOptions GetSchemaSerializerOptions() => new()
        {
            TypeInfoResolver = SerializerContext.Default
        };

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

        [JsonSerializable(typeof(ProviderSchema), GenerationMode = JsonSourceGenerationMode.Serialization)]
        [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true)]
        private partial class SerializerContext : JsonSerializerContext { }
    }
}
