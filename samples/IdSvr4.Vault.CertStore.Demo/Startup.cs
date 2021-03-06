﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdSvr4.Vault.CertStore.Demo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using System.Security.Claims;
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.Test;

    using IdentityServer4.Contrib.Vault.CertificateStore;
    using IdentityServer4.Contrib.Vault.CertificateStore.Options;
    using Microsoft.Extensions.Configuration;

    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            services.AddMvc();

            services.AddIdentityServer()
                .AddTestUsers(Users.Get())
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(IdentityResources.Get())
                .AddVaultAppRoleCertificateStore(new VaultCertificateStoreAppRoleOptions
                {
                    RoleId = Configuration.GetValue<string>("AppRoleId"),
                    SecretId = Configuration.GetValue<string>("AppSecretId"),

                    RoleName = "identity-server",
                    CommonName = "idsvr.test.com"
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
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
                    ClientId = Startup.Configuration.GetValue<string>("ServiceName"),
                    Enabled = true,

                    AccessTokenType = AccessTokenType.Jwt,
                    
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "service1",
                        "email"
                    },

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5001/auth/IdentityServer"
                    },

                    RequireConsent = false
                }
            };
        }
    }

    class IdentityResources
    {
        public static IList<IdentityResource> Get()
        {
            return new List<IdentityResource>
            {
                new IdentityServer4.Models.IdentityResources.OpenId(),
                new IdentityServer4.Models.IdentityResources.Profile(),
                new IdentityServer4.Models.IdentityResources.Email(),
                new IdentityResource
                {
                    Name = Startup.Configuration.GetValue<string>("ServiceName"),
                    Enabled = true,
                    UserClaims = new List<string>
                    {
                        "ServiceStack.Api.SelfHost.Role",
                        "ServiceStack.Api.SelfHost.Permission"
                    }                    
                }
            };
        }
    }

    class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = new Guid("93797286-1c85-483a-b238-fe09cd40f210").ToString(),
                    Username = "test@test.com",
                    Password = "password123",                    
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "test@test.com"),
                        new Claim(JwtClaimTypes.GivenName, "Boaby"),
                        new Claim(JwtClaimTypes.FamilyName, "Fyffe"),

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
