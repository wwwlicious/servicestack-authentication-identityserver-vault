// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdSvr3.Vault.CertStore.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security.Claims;
    using IdentityServer3.Contrib.Vault.CertificateStore;
    using IdentityServer3.Contrib.Vault.CertificateStore.Options;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Models;
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
            options.AddVaultAppRoleCertificateStore(new VaultCertificateStoreAppRoleOptions
            {
                RoleId = ConfigurationManager.AppSettings["AppRoleId"],
                SecretId = ConfigurationManager.AppSettings["AppSecretId"],
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
                    ClientId = ConfigurationManager.AppSettings["ServiceName"],
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
                    Name = ConfigurationManager.AppSettings["ServiceName"],
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(IdentityServer3.Core.Constants.ClaimTypes.Subject),
                        new ScopeClaim(IdentityServer3.Core.Constants.ClaimTypes.PreferredUserName)
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
