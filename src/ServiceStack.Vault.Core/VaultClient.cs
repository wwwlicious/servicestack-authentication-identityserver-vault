// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core
{
    using Interfaces;

    public class VaultClient : IVaultClient
    {
        public VaultClient(string vaultUri, IVaultAuth vaultAuth)
            : this(new VaultClientUri(vaultUri), vaultAuth)
        {            
        }

        internal VaultClient(IVaultClientUri vaultUri, IVaultAuth vaultAuth)
        {
            vaultUri.ThrowIfNull(nameof(vaultUri));
            vaultAuth.ThrowIfNull(nameof(vaultAuth));

            VaultUri = vaultUri;
            VaultAuth = vaultAuth;
        }

        public IVaultClientUri VaultUri { get; }

        public IVaultAuth VaultAuth { get;  }

        public IJsonServiceClient ServiceClient
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
