// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using Core.Helpers;
    using IdentityServer3.Core.Configuration;
    using Interfaces;

    public class VaultCertificateService : IVaultCertificateService
    {
        private readonly IdentityServerOptions options;
        private readonly IVaultCertificateStore vaultClient;
        private readonly IX509Certificate2Helper certificate2Helper;
        private readonly IRSACryptoServiceProviderHelper cryptoServiceProviderHelper;

        /// <summary>Constructor</summary>
        /// <param name="options">Identity Server Options</param>
        /// <param name="vaultClient">Vault Client</param>
        /// <param name="certificateHelper">Certificate Helper</param>
        /// <param name="cryptoServiceProviderHelper">Crypto Service Provider Helper</param>
        public VaultCertificateService(
            IdentityServerOptions options, 
            IVaultCertificateStore vaultClient, 
            IX509Certificate2Helper certificateHelper,
            IRSACryptoServiceProviderHelper cryptoServiceProviderHelper)
        {
            this.options = options.ThrowIfNull(nameof(options));
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
            this.certificate2Helper = certificateHelper.ThrowIfNull(nameof(certificateHelper));
            this.cryptoServiceProviderHelper = cryptoServiceProviderHelper.ThrowIfNull(nameof(cryptoServiceProviderHelper));
        }

        /// <summary>
        /// Get primary and secondary certificates from Vault if they don't exist or have expired
        /// </summary>
        /// <returns>List of Certificates</returns>
        public IEnumerable<X509Certificate2> GetCertificates()
        {
            if (options.SigningCertificate == null || options.SigningCertificate.NotAfter < DateTime.Now)
            {
                if (options.SecondarySigningCertificate != null && options.SecondarySigningCertificate.NotAfter > DateTime.Now)
                {
                    options.SigningCertificate = options.SecondarySigningCertificate;
                    options.SecondarySigningCertificate = null;
                }
                else
                {
                    options.SigningCertificate = GetNewCertificateFromVault();
                }
            }

            if (options.SecondarySigningCertificate == null || options.SecondarySigningCertificate.NotAfter < DateTime.Now)
            {
                options.SecondarySigningCertificate = GetNewCertificateFromVault();
            }

            return new List<X509Certificate2>
            {
                options.SigningCertificate, options.SecondarySigningCertificate
            };
        }

        /// <summary>Get Certificate from Vault then Transform to X509Certificate2 format</summary>
        /// <returns></returns>
        private X509Certificate2 GetNewCertificateFromVault()
        {
            var vaultKey = vaultClient.GetCertificate();
            var privateKeyProvider = cryptoServiceProviderHelper.GetPrivateKeyProvider(vaultKey.PrivateKey);
            var certificate = certificate2Helper.GetCertificate(vaultKey.Certificate);
            certificate.PrivateKey = privateKeyProvider;

            var certIdentifier = Guid.NewGuid();

            // Need to write the certificate to file with the private key then read it back
            // from a file for the JTokenHandler library to be able to sign the tokens
            certificate2Helper.WriteCertificateToFile($"Idsvr-{certIdentifier}.pfx", certificate);

            return certificate2Helper.LoadCertificate($"Idsvr-{certIdentifier}.pfx");
        }
    }
}
