// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore
{
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
        public static void AddVaultCertificateStore(this IdentityServerOptions options, VaultCertificateStoreAppIdOptions vaultOptions)
        {
            options.AddVaultCertificateStore(vaultOptions, new VaultAppIdAuth(vaultOptions.AppId, vaultOptions.UserId));
        }

        private static void AddVaultCertificateStore(this IdentityServerOptions options, VaultCertificateStoreOptions vaultOptions, IVaultAuth vaultAuth)
        {
            // This isn't great but we need a cert at startup
            var client = new VaultClient(vaultOptions.VaultUrl, vaultAuth);
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
