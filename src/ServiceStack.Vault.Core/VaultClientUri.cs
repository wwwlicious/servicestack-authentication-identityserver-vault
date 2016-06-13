// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core
{
    using System;
    using Interfaces;

    public class VaultClientUri : IVaultClientUri
    {
        public VaultClientUri(string vaultUri)
        {
            vaultUri.ThrowIfNullOrEmpty(nameof(vaultUri));

            VaultUri = vaultUri;

            ServiceClientFunc = () => new JsonServiceClient(VaultUri);
        }

        public Func<IJsonServiceClient> ServiceClientFunc { get; set; } 

        public string VaultUri { get; }

        public IJsonServiceClient ServiceClient => ServiceClientFunc();
    }
}
