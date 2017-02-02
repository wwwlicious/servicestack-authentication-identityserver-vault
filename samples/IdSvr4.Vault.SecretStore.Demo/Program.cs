namespace IdSvr4.Vault.SecretStore.Demo
{
    using System;
    using System.IO;
    using System.Threading;
    using Microsoft.AspNetCore.Hosting;
    using Serilog;

    public class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        public static string IdentityServerAppId = "146a3d05-2042-4855-93ba-1b122e70eb6d";
        public static string IdentityServerUserId = "976c1095-a7b4-4b6f-8cd8-d71d860c6a31";

        public static string ServiceId = "service1";
        public static string ServiceAppId = "f8a5a40f-ecd9-43da-a009-82f180e1ef84";
        public static string ServiceUserId = "27ded1df-7aca-40ba-a825-cc9bf5cb7f88";

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

            // 4. Create list of client secrets for the micro-service
            VaultUrl.CreateSecrets(rootToken, ServiceId, new[] { "secret1", "secret2", "secret3", "secret4", "secret5" });

            // 5. Create AppRole for the client

            // 6. Create AppRole 

            //// 5. Create app-id and user-id for the client that only have access to the secret end point
            //VaultUrl.EnableAppId(rootToken);

            //// Create Identity Server app-id/user-id credentials
            //VaultUrl.CreateAppId(rootToken, IdentityServerAppId, "root");
            //VaultUrl.CreateUserId(rootToken, IdentityServerUserId);
            //VaultUrl.MapUserIdsToAppIds(rootToken, IdentityServerUserId, IdentityServerAppId);

            //// Create Service app-id/user-id credentials
            //VaultUrl.CreateAppId(rootToken, ServiceAppId, "root");
            //VaultUrl.CreateUserId(rootToken, ServiceUserId);
            //VaultUrl.MapUserIdsToAppIds(rootToken, ServiceUserId, ServiceAppId);


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
