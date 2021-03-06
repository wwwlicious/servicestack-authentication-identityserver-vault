﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System;
    using System.Linq;
    using Core;
    using Core.Interfaces;
    using Core.VaultAuth;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Options;
    using Stores;
    using Validation;

    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddClientDataStore(this IIdentityServerBuilder builder, Func<IServiceProvider, IClientDataStore> clientDataStore)
        {
            builder.Services.AddTransient(clientDataStore);
            return builder;
        }

        public static IIdentityServerBuilder AddResourceDataStore(this IIdentityServerBuilder builder, Func<IServiceProvider, IResourceDataStore> resourceDataStore)
        {
            builder.Services.AddTransient(resourceDataStore);
            return builder;
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static IIdentityServerBuilder AddVaultAppIdClientSecretStore(
            this IIdentityServerBuilder builder,
            VaultClientSecretStoreAppIdOptions vaultOptions)
        {
            builder.Services.AddSingleton<IVaultAuth>(resolver => new VaultAppIdAuth(vaultOptions.AppId, vaultOptions.UserId, resolver.GetService<ILogger<VaultAppIdAuth>>()));
            return builder.AddVaultClientSecretStore(vaultOptions);
        }

        public static IIdentityServerBuilder AddVaultAppRoleClientSecretStore(
            this IIdentityServerBuilder builder,
            VaultClientSecretStoreAppRoleOptions vaultOptions)
        {
            builder.Services.AddSingleton<IVaultAuth>(resolver => new VaultAppRoleAuth(vaultOptions.RoleId, vaultOptions.SecretId, resolver.GetService<ILogger<VaultAppRoleAuth>>()));
            return builder.AddVaultClientSecretStore(vaultOptions);
        }

        private static IIdentityServerBuilder AddVaultClientSecretStore(
            this IIdentityServerBuilder builder,
            VaultClientSecretStoreOptions vaultOptions)
        {
            builder.Services.AddSingleton<IVaultSecretStore>(resolver => new VaultSecretStore(new VaultClient(resolver.GetService<IVaultAuth>(), vaultOptions.VaultUrl, vaultOptions.VaultCertificate), resolver.GetService<ILogger<VaultSecretStore>>()));

            builder.Services.AddTransient<IClientStore>(resolver => new ClientSecretStore(resolver.GetService<IVaultSecretStore>(), resolver.GetService<IClientDataStore>()));
            builder.Services.AddTransient<IResourceStore>(resolver => new ResourceSecretStore(resolver.GetService<IVaultSecretStore>(), resolver.GetService<IResourceDataStore>()));

            var basicParsers = builder.Services.Where(IsSecretParser<BasicAuthenticationSecretParser>).ToArray();
            foreach (var basicParser in basicParsers)
            {
                builder.Services.Remove(basicParser);
            }

            var bodyParsers = builder.Services.Where(IsSecretParser<PostBodySecretParser>).ToArray();
            foreach (var bodyParser in bodyParsers)
            {
                builder.Services.Remove(bodyParser);
            }

            builder.AddSecretParser<VaultPostBodySecretParser>();
            builder.AddSecretParser<VaultBasicAuthenticationSecretParser>();

            builder.Services.AddTransient<ISecretValidator, VaultSecretValidator>();

            return builder;
        }

        private static bool IsSecretParser<T>(ServiceDescriptor descriptor)
        {
            return descriptor != null && descriptor.ServiceType == typeof(ISecretParser) && descriptor.ImplementationType == typeof(T);
        }
    }
}
