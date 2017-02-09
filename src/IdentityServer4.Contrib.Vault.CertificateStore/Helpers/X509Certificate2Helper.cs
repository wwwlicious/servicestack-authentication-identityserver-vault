// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using Core.Helpers;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    public class X509Certificate2Helper : IX509Certificate2Helper
    {
        private readonly ILogger<X509Certificate2Helper> logger;

        public X509Certificate2Helper(ILogger<X509Certificate2Helper> logger)
        {            
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public X509Certificate2 CreateCertificate(byte[] certificateData)
        {
            var file = Path.Combine(Path.GetTempPath(), "Idsvr" + Guid.NewGuid());
            X509Certificate2 certificate;
            try
            {
                logger.LogDebug($"Writing certificate bytes to temporary file at {file}");
                File.WriteAllBytes(file, certificateData);

                logger.LogDebug($"Creating certificate from temporary file at {file}");
                certificate = new X509Certificate2(file, "",
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
            catch (Exception exception)
            {
                logger.LogError($"Unable to create certificate from {file}", exception);
                throw;
            }
            finally
            {
                logger.LogDebug($"Cleaning up temporary file at {file}");
                File.Delete(file);
            }
            return certificate;
        }
    }
}