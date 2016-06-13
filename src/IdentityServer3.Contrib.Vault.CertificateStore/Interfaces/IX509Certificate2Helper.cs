// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Interfaces
{
    using System.Security.Cryptography.X509Certificates;

    public interface IX509Certificate2Helper
    {
        /// <summary>Writes the Certificate to a temporary file then creates the Certificate from there</summary>
        /// <param name="certificateData">Vault Certificate Data</param>
        /// <returns>X509 Certificate 2</returns>
        X509Certificate2 GetCertificate(byte[] certificateData);

        /// <summary>
        /// Write the Certificate and Private Key to a disk
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="certificate">X509 Certificate to write to disk</param>
        void WriteCertificateToFile(string fileName, X509Certificate2 certificate);

        /// <summary>
        /// Load the Certificate and Private Key then cleanup
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>X509 Certificate from disk</returns>
        X509Certificate2 LoadCertificate(string fileName);
    }
}
