// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore.Options
{
    using System.Security.Cryptography.X509Certificates;

    public abstract class VaultClientSecretStoreOptions
    {
        protected VaultClientSecretStoreOptions()
        {
            VaultUrl = "http://127.0.0.1:8200";
        }

        public string VaultUrl { get; set; }

        public X509Certificate2 VaultCertificate { get; set; }
    }
}
