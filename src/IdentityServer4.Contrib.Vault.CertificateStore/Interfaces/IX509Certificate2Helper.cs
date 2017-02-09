// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore.Interfaces
{
    using System.Security.Cryptography.X509Certificates;

    public interface IX509Certificate2Helper
    {
        X509Certificate2 CreateCertificate(byte[] certificateData);
    }
}
