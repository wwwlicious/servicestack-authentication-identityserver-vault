// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using Core.Helpers;
    using IdentityServer3.Core.Logging;
    using Interfaces;
    using Options;
    using Org.BouncyCastle.Security;

    public class X509Certificate2Helper : IX509Certificate2Helper
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly VaultCertificateStoreOptions options;

        public X509Certificate2Helper(VaultCertificateStoreOptions options)
        {
            this.options = options.ThrowIfNull(nameof(options));
        }

        /// <summary>Writes the Certificate to a temporary file then creates the Certificate from there</summary>
        /// <param name="certificateData">Vault Certificate Data</param>
        /// <returns>X509 Certificate 2</returns>
        public X509Certificate2 GetCertificate(byte[] certificateData)
        {
            var file = Path.Combine(options.CertificateTempPath, "Idsvr" + Guid.NewGuid());
            X509Certificate2 certificate;
            try
            {
                Logger.Debug($"Writing certificate bytes to temporary file at {file}");
                File.WriteAllBytes(file, certificateData);

                Logger.Debug($"Creating certificate from temporary file at {file}");
                certificate = new X509Certificate2(file, "",
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => $"Unable to create certificate from {file}", exception);
                throw;
            }
            finally
            {
                Logger.Debug($"Cleaning up temporary file at {file}");
                File.Delete(file);
            }
            return certificate;
        }

        /// <summary>
        /// Write the Certificate and Private Key to a disk
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="certificate">X509 Certificate to write to disk</param>
        public void WriteCertificateToFile(string fileName, X509Certificate2 certificate)
        {
            try
            {
                Logger.Debug($"Writing pfx certificate to {fileName}");
                File.WriteAllBytes(fileName, certificate.Export(X509ContentType.Pfx, (string)null));
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => $"Unable to write file {fileName}", exception);
                throw;
            }
        }

        /// <summary>
        /// Load the Certificate and Private Key then cleanup
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>X509 Certificate from disk</returns>
        public X509Certificate2 LoadCertificate(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    var certificate = new X509Certificate2(bytes);
                    if (!certificate.HasPrivateKey)
                    {
                        throw new InvalidKeyException("Private key not set");
                    }
                    return certificate;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => $"Unable to load certificate from file {fileName}", exception);
                throw;
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
