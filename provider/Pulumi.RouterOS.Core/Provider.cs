using Pulumi.Experimental.Provider;

namespace Pulumi.RouterOS.Core
{
    public partial class Provider(IHost providerHost) : Experimental.Provider.Provider
    {
        private readonly IHost _providerHost = providerHost ?? throw new ArgumentNullException(nameof(providerHost));

        public override Task<GetSchemaResponse> GetSchema(GetSchemaRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetSchemaResponse
            {
                Schema = "{ \"name\":\"routeros\" }"
            });
        }
    }
}
