// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Core.Helpers;
    using Interfaces;
    using Microsoft.IdentityModel.Tokens;

    public class VaultCertificateService : IVaultCertificateService
    {
        private readonly IVaultCertificateStore vaultClient;
        private readonly IX509Certificate2Helper certificate2Helper;

        private X509Certificate2 certificate;

        public VaultCertificateService(
            IVaultCertificateStore vaultClient,
            IX509Certificate2Helper certificateHelper)
        {
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
            this.certificate2Helper = certificateHelper.ThrowIfNull(nameof(certificateHelper));
        }

        public SigningCredentials SigningCredentials
        {
            get
            {
                GetNewCertificateFromVault();
                return new SigningCredentials(new X509SecurityKey(certificate), "RS256");
            }
        }

        private bool IsValidCertificate()
        {
            return certificate != null && certificate.NotAfter > DateTime.Now;
        }

        /// <summary>Get Certificate from Vault then Transform to X509Certificate2 format</summary>
        /// <returns></returns>
        private void GetNewCertificateFromVault()
        {
            if (IsValidCertificate()) return;

            var vaultKey = vaultClient.GetCertificate();

            certificate = certificate2Helper.CreateCertificate(vaultKey.Certificate, vaultKey.PrivateKey);        
        }
    }
}
