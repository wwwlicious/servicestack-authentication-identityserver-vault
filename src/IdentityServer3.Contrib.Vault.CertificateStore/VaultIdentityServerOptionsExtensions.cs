// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore
{
    using System;
    using Core;
    using Core.Interfaces;
    using Core.VaultAuth;
    using Helpers;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Services;
    using Interfaces;
    using Options;

    public static class VaultIdentityServerOptionsExtensions
    {
        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static void AddVaultCertificateStore(this IdentityServerOptions options, VaultCertificateStoreAppIdOptions vaultOptions)
        {
            options.AddVaultCertificateStore(vaultOptions, new VaultAppIdAuth(vaultOptions.AppId, vaultOptions.UserId));
        }

        public static void AddVaultAppRoleCertificateStore(this IdentityServerOptions options, VaultCertificateStoreAppRoleOptions vaultOptions)
        {
            options.AddVaultCertificateStore(vaultOptions, new VaultAppRoleAuth(vaultOptions.RoleId, vaultOptions.SecretId));
        }

        private static void AddVaultCertificateStore(this IdentityServerOptions options, VaultCertificateStoreOptions vaultOptions, IVaultAuth vaultAuth)
        {
            // This isn't great but we need a cert at startup
            var client = new VaultClient(vaultAuth, vaultOptions.VaultUrl, vaultOptions.VaultCertificate);
            var certificateStore = new VaultCertificateStore(client, vaultOptions.RoleName, vaultOptions.CommonName);
            var certificateHelper = new X509Certificate2Helper();
            var privateKeyHelper = new RsaCryptoServiceProviderHelper();
            var vaultService = new VaultCertificateService(options, certificateStore, certificateHelper, privateKeyHelper);
            vaultService.GetCertificates();

            // Register our dependencies
            options.Factory.Register(new Registration<IVaultCertificateService>(vaultService));
            options.Factory.SigningKeyService = new Registration<ISigningKeyService, VaultTokenSigningKeyService>();
        }
    }
}
