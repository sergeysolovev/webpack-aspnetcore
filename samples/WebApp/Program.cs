using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration.CommandLine;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var cliProvider = new CommandLineConfigurationProvider(args);
            cliProvider.Load();
            cliProvider.TryGet("startup", out var startUpArg);

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup(getStartupType())
                .Build();

            System.Type getStartupType() =>
                startUpArg == "withPathBase" ? typeof(StartupWithPathBase) :
                startUpArg == "withStaticFileOpts" ? typeof(StartupWithStaticFileOptions) :
                typeof(Startup);
        }
    }
}
