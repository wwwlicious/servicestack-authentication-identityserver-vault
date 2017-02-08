namespace IdSvr4.Vault.SecretStore.Demo
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Serilog;

    public class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo
               // .LiterateConsole(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
               .Seq(serverUrl: "http://localhost:5341")
               .CreateLogger();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
