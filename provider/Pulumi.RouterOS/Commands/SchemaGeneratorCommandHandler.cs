using System.CommandLine.Invocation;

namespace Pulumi.RouterOS.Commands
{
    internal interface ISchemaProviderCommandHandler : ICommandHandler { }

    internal class SchemaProviderCommandHandler : ISchemaProviderCommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotSupportedException("Use InvokeAsync instead of Invoke");
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
