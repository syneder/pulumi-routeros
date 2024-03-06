using Microsoft.Extensions.DependencyInjection;
using Pulumi.RouterOS.Commands;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Pulumi.RouterOS
{
    internal class Program
    {
        public static Task<int> Main(string[] args)
        {
            var services = Startup.ConfigureServices(new ServiceCollection());
            var serviceProvider = services.BuildServiceProvider();

            var schemaCommand = new Command("schema")
            {
                Description = "Get Pulumi package schema",
                Handler = serviceProvider.GetRequiredService<ISchemaProviderCommandHandler>()
            };

            var command = new RootCommand
            {
                Description = "Launch Pulumi Provider to manage RouterOS resources",
                Handler = serviceProvider.GetRequiredService<ICommandHandler>()
            };

            command.AddCommand(schemaCommand);

            var builder = new CommandLineBuilder(command)
                .CancelOnProcessTermination()
                .UseExceptionHandler()
                .UseHelp()
                .UseVersionOption();

            return builder.Build().InvokeAsync(args);
        }
    }
}
