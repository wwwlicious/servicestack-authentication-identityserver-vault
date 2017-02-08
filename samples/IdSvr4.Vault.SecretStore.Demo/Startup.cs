namespace IdSvr4.Vault.SecretStore.Demo
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
    using IdentityServer4.Contrib.Vault.ClientSecretStore;
    using IdentityServer4.Contrib.Vault.ClientSecretStore.Interfaces;
    using IdentityServer4.Contrib.Vault.ClientSecretStore.Options;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using IdentityServer4.Test;
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
                    .AddTemporarySigningCredential()
                    .AddTestUsers(Users.Get())
                    .AddClientDataStore(resolver => new InMemoryClientDataStore())
                    .AddResourceDataStore(resolver => new InMemoryResourceDataStore())
                    .AddVaultAppRoleClientSecretStore(new VaultClientSecretStoreAppRoleOptions
                    {
                        RoleId = Configuration.GetValue<string>("AppRoleId"),
                        SecretId = Configuration.GetValue<string>("AppSecretId")                     
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

    class InMemoryClientDataStore : InMemoryClientStore, IClientDataStore
    {
        public InMemoryClientDataStore()
            : base(Clients.Get())
        {
        }
    }

    class InMemoryResourceDataStore : InMemoryResourcesStore, IResourceDataStore
    {
        public InMemoryResourceDataStore()
            : base(IdentityResources.Get(), new List<ApiResource>())
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
                    ClientId = Startup.Configuration.GetValue<string>("ServiceName"),
                    Enabled = true,

                    AccessTokenType = AccessTokenType.Jwt,

                    AllowOfflineAccess = true,

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5001/auth/IdentityServer"
                    },

                    RequireConsent = false,

                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "service1",
                        "email"
                    }
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
