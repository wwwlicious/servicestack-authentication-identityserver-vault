// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using Core;
    using Helpers;
    using Core.Interfaces;
    using Core.VaultAuth;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Services;
    using Interfaces;
    using Options;

    public static class VaultIdentityServerServiceFactoryExtensions
    {
        public static void RegisterClientDataStore(this IdentityServerServiceFactory factory, Registration<IClientDataStore> registration)
        {
            factory.Register(registration);
        }

        public static void RegisterScopeDataStore(this IdentityServerServiceFactory factory, Registration<IScopeDataStore> registration)
        {
            factory.Register(registration);
        }

        public static void AddVaultClientSecretStore(
            this IdentityServerServiceFactory factory,
            VaultClientSecretStoreAppIdOptions vaultOptions)
        {
            factory.AddVaultClientSecretStore(vaultOptions, new VaultAppIdAuth(vaultOptions.AppId, vaultOptions.UserId));
        }

        /// <summary>Setup Vault to store Client Secrets</summary>
        /// <param name="factory">Identity Server Service Factory</param>
        /// <param name="vaultOptions">Vault Options</param>
        /// <param name="vaultAuth">Vault Authentication</param>
        private static void AddVaultClientSecretStore(
            this IdentityServerServiceFactory factory,
            VaultClientSecretStoreOptions vaultOptions,
            IVaultAuth vaultAuth)
        {
            factory.Register(new Registration<IVaultSecretStore>(new VaultSecretStore(new VaultClient(vaultAuth, vaultOptions.VaultUrl, vaultOptions.VaultCertificate))));

            factory.ClientStore = new Registration<IClientStore>(resolver => new ClientSecretStore(resolver.Resolve<IVaultSecretStore>(), resolver.Resolve<IClientDataStore>()));
            factory.ScopeStore = new Registration<IScopeStore>(resolver => new ScopeSecretStore(resolver.Resolve<IVaultSecretStore>(), resolver.Resolve<IScopeDataStore>()));

            factory.Register(new Registration<IRequestParser, RequestParser>());

            factory.SecretValidators.Clear();
            factory.SecretValidators.Add(new Registration<ISecretValidator, VaultSecretValidator>());

            factory.SecretParsers.Clear();
            factory.SecretParsers.Add(new Registration<ISecretParser, VaultPostBodySecretParser>());
            factory.SecretParsers.Add(new Registration<ISecretParser, VaultBasicAuthenticationSecretParser>());
        }
    }
}
