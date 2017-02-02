namespace IdSvr4.Vault.CertStore.Demo
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using System.Threading;
    using Serilog;

    public class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        public static string ServiceId = "service1";

        public static string AppRoleId;
        public static string AppRoleSecretId;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo
               // .LiterateConsole(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
               .Seq(serverUrl: "http://localhost:5341")
               .CreateLogger();

            // 1. Initialize vault.
            string rootToken;
            string[] keys;
            Console.WriteLine($"Initializing vault at {VaultUrl}");
            VaultUrl.Initialize(out rootToken, out keys);

            // 2. Unseal vault
            Console.WriteLine($"Unsealing vault at {VaultUrl}");
            VaultUrl.Unseal(keys);

            Thread.Sleep(1000);

            // 3. Create transit end-point for encryption / decryption keys
            Console.WriteLine("Mount transit backend to create vault encryption keys");
            VaultUrl.MountTransit(rootToken);
            Console.WriteLine("Create encryption token for encrypting/decrypting secrets");
            VaultUrl.CreateEncryptionKey(rootToken, ServiceId);

            // 3.a Create PKI end-point for certificatey "stuff"
            VaultUrl.MountPki(rootToken);
            VaultUrl.MountTunePki(rootToken);
            VaultUrl.GenerateRootCertificate(rootToken, "test.com", "87600h");
            VaultUrl.SetCertificateUrlConfiguration(rootToken);
            VaultUrl.GetCertificateUrlConfiguration(rootToken);
            VaultUrl.GenerateCertificateRole(rootToken, "identity-server", "test.com");

            // 4. Create the policy
            VaultUrl.CreatePolicy(rootToken, "identity-server-cert", "pki/issue/identity-server", new [] { "create", "read", "update" });

            // 5. Create the AppRole
            VaultUrl.EnableAppRole(rootToken);
            VaultUrl.CreateAppRole(rootToken, "identity-server", new []{ "identity-server-cert" });

            AppRoleId = VaultUrl.GetAppRoleId(rootToken, "identity-server");
            AppRoleSecretId = VaultUrl.GetAppRoleSecretId(rootToken, "identity-server");
            
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
