// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdSvr3.Vault.SecretStore.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using IdentityServer3.Contrib.Vault.ClientSecretStore;
    using IdentityServer3.Contrib.Vault.ClientSecretStore.Interfaces;
    using IdentityServer3.Contrib.Vault.ClientSecretStore.Options;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Services.InMemory;
    using Microsoft.Owin.Hosting;
    using Owin;

    class Program
    {
        private static string VaultUrl = "http://127.0.0.1:8200";

        static void Main(string[] args)
        {
            IDisposable webApp = null;

            try
            {
                // Startup Identity Server
                webApp = WebApp.Start<Startup>("http://localhost:5000");

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

            factory.AddVaultClientSecretStore(new VaultClientSecretStoreAppRoleOptions
            {
                RoleId = ConfigurationManager.AppSettings["AppRoleId"],
                SecretId = ConfigurationManager.AppSettings["AppSecretId"]
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
            using (var stream = typeof(Startup).Assembly.GetManifestResourceStream("IdSvr3.Vault.SecretStore.Demo.idsrv3test.pfx"))
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
                    ClientId = ConfigurationManager.AppSettings["ServiceName"],
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
                    Name = ConfigurationManager.AppSettings["ServiceName"],
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
