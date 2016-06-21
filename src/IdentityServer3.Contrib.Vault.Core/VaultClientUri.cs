// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Helpers;
    using Interfaces;

    public class VaultClientUri : IVaultClientUri
    {
        public VaultClientUri(string vaultUri, X509Certificate2 cert)
        {
            VaultUri = vaultUri.ThrowIfNullOrEmpty(nameof(vaultUri));
            ServiceClientFunc = () => new VaultHttpClient(VaultUri, cert);
        }

        public Func<IVaultHttpClient> ServiceClientFunc { get; set; }

        public string VaultUri { get; }

        public IVaultHttpClient ServiceClient => ServiceClientFunc();
    }
}
