// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core
{
    using System;
    using Helpers;
    using Interfaces;

    public class VaultClientUri : IVaultClientUri
    {
        public VaultClientUri(string vaultUri)
        {
            VaultUri = vaultUri.ThrowIfNullOrEmpty(nameof(vaultUri));
            ServiceClientFunc = () => new VaultHttpClient(VaultUri);
        }

        public Func<IVaultHttpClient> ServiceClientFunc { get; set; }

        public string VaultUri { get; }

        public IVaultHttpClient ServiceClient => ServiceClientFunc();
    }
}
