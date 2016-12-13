// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using global::ServiceStack.Text;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Services.InMemory;
    using Interfaces;
    using Microsoft.Owin.Hosting;
    using Options;
    using Owin;

    class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        public static string IdentityServerAppId = "146a3d05-2042-4855-93ba-1b122e70eb6d";
        public static string IdentityServerUserId = "976c1095-a7b4-4b6f-8cd8-d71d860c6a31";

        static void Main(string[] args)
        {
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
            VaultUrl.CreateEncryptionKey(rootToken, AppHost.ServiceId);

            // 3.a Create PKI end-point for certificatey "stuff"
            VaultUrl.MountPki(rootToken);
            VaultUrl.MountTunePki(rootToken);
            VaultUrl.GenerateRootCertificate(rootToken, "test.com", "87600h");
            VaultUrl.SetCertificateUrlConfiguration(rootToken);
            VaultUrl.GetCertificateUrlConfiguration(rootToken);
            VaultUrl.GenerateCertificateRole(rootToken, "identity-server", "test.com");

            // 4. Create list of client secrets for the micro-service
            VaultUrl.CreateSecrets(rootToken, AppHost.ServiceId, new[] { "secret1", "secret2", "secret3", "secret4", "secret5" });

            // 5. Create app-id and user-id for the client that only have access to the secret end point
            VaultUrl.EnableAppId(rootToken);

            // Create Identity Server app-id/user-id credentials
            VaultUrl.CreateAppId(rootToken, IdentityServerAppId, "root");
            VaultUrl.CreateUserId(rootToken, IdentityServerUserId);
            VaultUrl.MapUserIdsToAppIds(rootToken, IdentityServerUserId, IdentityServerAppId);

            // Create Service app-id/user-id credentials
            VaultUrl.CreateAppId(rootToken, AppHost.ServiceAppId, "root");
            VaultUrl.CreateUserId(rootToken, AppHost.ServiceUserId);
            VaultUrl.MapUserIdsToAppIds(rootToken, AppHost.ServiceUserId, AppHost.ServiceAppId);

            IDisposable webApp = null;

            try
            {
                // Startup Identity Server
                webApp = WebApp.Start<Startup>("http://localhost:5000");

                // Now start up service stack client
                new AppHost("http://localhost:5001/").Init().Start("http://*:5001/");
                "ServiceStack Self Host with Razor listening at http://localhost:5001 ".Print();
                Process.Start("http://localhost:5001/");
                
                Console.ReadLine();
            }
            finally
            {
                webApp?.Dispose();
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = new IdentityServerServiceFactory().UseInMemoryUsers(Users.Get());

            factory.UserService = new Registration<IUserService, UserService>();

            factory.RegisterClientDataStore(new Registration<IClientDataStore>(resolver => new InMemoryClientDataStore(Clients.Get())));
            factory.RegisterScopeDataStore(new Registration<IScopeDataStore>(resolver => new InMemoryScopeDataStore(Scopes.Get())));
            factory.AddVaultClientSecretStore(
                new VaultClientSecretStoreAppIdOptions
                {
                    AppId = "146a3d05-2042-4855-93ba-1b122e70eb6d",
                    UserId = "976c1095-a7b4-4b6f-8cd8-d71d860c6a31"
                });

            var options = new IdentityServerOptions
            {
                SigningCertificate = LoadCertificate(),

                Factory = factory,
                RequireSsl = false
            };

            app.UseIdentityServer(options);
        }

        private X509Certificate2 LoadCertificate()
        {
            using (var stream = typeof(Startup).Assembly.GetManifestResourceStream("IdentityServer3.Contrib.Vault.ClientSecretStore.Demo.idsrv3test.pfx"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return new X509Certificate2(bytes, "idsrv3test");
            }
        }
    }

    class InMemoryClientDataStore : InMemoryClientStore, IClientDataStore
    {
        public InMemoryClientDataStore(IEnumerable<Client> clients) 
            : base(clients)
        {

        }
    }

    class InMemoryScopeDataStore : InMemoryScopeStore, IScopeDataStore
    {
        public InMemoryScopeDataStore(IEnumerable<Scope> scopes) 
            : base(scopes)
        {
        }
    }

    class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "ServiceStack.Vault.ClientSecrets.Demo",
                    ClientId = "service1",
                    Enabled = true,

                    AccessTokenType = AccessTokenType.Jwt,

                    Flow = Flows.Hybrid,

                    AllowAccessToAllScopes = true,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5001/auth/IdentityServer"
                    },

                    RequireConsent = false
                }
            };
        }
    }

    class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>(StandardScopes.All)
            {
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Enabled = true,
                    Name = "service1",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(IdentityServer3.Core.Constants.ClaimTypes.Subject),
                        new ScopeClaim(IdentityServer3.Core.Constants.ClaimTypes.PreferredUserName)
                    }
                }
            };
        }
    }

    class UserService : InMemoryUserService
    {
        public UserService(List<InMemoryUser> users)
            : base(users)
        {

        }
    }

    class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = new Guid("93797286-1c85-483a-b238-fe09cd40f210").ToString(),
                    Username = "test@test.com",
                    Password = "password123",
                    Enabled = true,
                    Claims = new List<Claim>
                    {
                        new Claim(IdentityServer3.Core.Constants.ClaimTypes.Email, "test@test.com"),
                        new Claim(IdentityServer3.Core.Constants.ClaimTypes.GivenName, "Boaby"),
                        new Claim(IdentityServer3.Core.Constants.ClaimTypes.FamilyName, "Fyffe"),

                        new Claim("ServiceStack.Api.SelfHost.Role", "Manager"),
                        new Claim("ServiceStack.Api.SelfHost.Role", "Buyer"),

                        new Claim("ServiceStack.Api.SelfHost.Permission", "CanSeeAllOrders"),
                        new Claim("ServiceStack.Api.SelfHost.Permission", "CanBuyStuff")
                    }
                }
            };
        }
    }
}
