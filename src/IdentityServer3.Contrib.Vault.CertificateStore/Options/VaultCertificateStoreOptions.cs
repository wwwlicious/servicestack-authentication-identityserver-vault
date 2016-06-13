// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Options
{
    public abstract class VaultCertificateStoreOptions
    {
        protected VaultCertificateStoreOptions()
        {
            VaultUrl = "http://127.0.0.1:8200";
        }

        public string VaultUrl { get; set; }

        public string RoleName { get; set; }

        public string CommonName { get; set; }
    }
}
