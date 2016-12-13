// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Options
{
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    public abstract class VaultCertificateStoreOptions
    {
        protected VaultCertificateStoreOptions()
        {
            VaultUrl = "http://127.0.0.1:8200";
            CertificateTempPath = Path.GetTempPath();
        }

        public string VaultUrl { get; set; }

        public X509Certificate2 VaultCertificate { get; set; }

        public string RoleName { get; set; }

        public string CommonName { get; set; }

        public string CertificateTempPath { get; set; }
    }
}
