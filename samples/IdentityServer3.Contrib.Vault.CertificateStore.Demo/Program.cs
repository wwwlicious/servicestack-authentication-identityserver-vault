// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.ServiceStack.Vault.CertificateStore.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Claims;
    using System.Threading;
    using Contrib.Vault.CertificateStore;
    using Contrib.Vault.CertificateStore.Options;
    using Core.Configuration;
    using Core.Models;
    using Core.Services.InMemory;
    using global::ServiceStack.Text;
    using Microsoft.Owin.Hosting;
    using Owin;
    using Constants = Core.Constants;

    class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        public static string IdentityServerAppId = "146a3d05-2042-4855-93ba-1b122e70eb6d";
        public static string IdentityServerUserId = "976c1095-a7b4-4b6f-8cd8-d71d860c6a31";

        public static string ServiceId = "service1";

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
            VaultUrl.CreateEncryptionKey(rootToken, ServiceId);

            // 3.a Create PKI end-point for certificatey "stuff"
            VaultUrl.MountPki(rootToken);
            VaultUrl.MountTunePki(rootToken);
            VaultUrl.GenerateRootCertificate(rootToken, "test.com", "87600h");
            VaultUrl.SetCertificateUrlConfiguration(rootToken);
            VaultUrl.GetCertificateUrlConfiguration(rootToken);
            VaultUrl.GenerateCertificateRole(rootToken, "identity-server", "test.com");

            // 5. Create app-id and user-id for the client that only have access to the secret end point
            VaultUrl.EnableAppId(rootToken);

            // Create Identity Server app-id/user-id credentials
            VaultUrl.CreateAppId(rootToken, IdentityServerAppId, "root");
            VaultUrl.CreateUserId(rootToken, IdentityServerUserId);
            VaultUrl.MapUserIdsToAppIds(rootToken, IdentityServerUserId, IdentityServerAppId);

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
            var factory = new IdentityServerServiceFactory()
                .UseInMemoryScopes(Scopes.Get())
                .UseInMemoryUsers(Users.Get())
                .UseInMemoryClients(Clients.Get());

            var options = new IdentityServerOptions
            {
                Factory = factory,
                RequireSsl = false
            };

            // Wire up Vault as being the X509 Certificate Signing Store
            options.AddVaultCertificateStore(new VaultCertificateStoreAppIdOptions
            {
                AppId = "146a3d05-2042-4855-93ba-1b122e70eb6d",
                UserId = "976c1095-a7b4-4b6f-8cd8-d71d860c6a31",
                RoleName = "identity-server",
                CommonName = "idsvr.test.com"
            });

            app.UseIdentityServer(options);
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

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    },

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
                        new ScopeClaim(Constants.ClaimTypes.Subject),
                        new ScopeClaim(Constants.ClaimTypes.PreferredUserName)
                    },
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    }
                }
            };
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
                        new Claim(Constants.ClaimTypes.Email, "test@test.com"),
                        new Claim(Constants.ClaimTypes.GivenName, "Boaby"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Fyffe"),

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
