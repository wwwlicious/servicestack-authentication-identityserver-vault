// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core
{
    using System.Security.Cryptography.X509Certificates;
    using Helpers;
    using Interfaces;

    public class VaultClient : IVaultClient
    {
        public VaultClient(IVaultAuth vaultAuth, string vaultUri, X509Certificate2 cert)
        {        
            VaultUri = new VaultClientUri(vaultUri, cert);
            VaultAuth = vaultAuth.ThrowIfNull(nameof(vaultAuth));
        }

        public VaultClient(IVaultClientUri vaultClientUri, IVaultAuth vaultAuth)
        {
            VaultUri = vaultClientUri.ThrowIfNull(nameof(vaultClientUri));
            VaultAuth = vaultAuth.ThrowIfNull(nameof(vaultAuth));
        }

        public IVaultAuth VaultAuth { get; }

        public IVaultClientUri VaultUri { get; }

        public IVaultHttpClient ServiceClient
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VaultAuth.AuthToken))
                {
                    VaultAuth.Authenticate(VaultUri);
                }

                var client = VaultUri.ServiceClient;
                client.AddHeader("X-Vault-Token", VaultAuth.AuthToken);
                return client;              
            }
        }
    }
}
