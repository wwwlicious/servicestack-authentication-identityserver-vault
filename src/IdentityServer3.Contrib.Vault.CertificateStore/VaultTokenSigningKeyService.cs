// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Core.Helpers;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Services;
    using Interfaces;

    public class VaultTokenSigningKeyService : ISigningKeyService
    {
        private readonly IdentityServerOptions options;
        private readonly IVaultCertificateService certService;

        public VaultTokenSigningKeyService(IdentityServerOptions options, IVaultCertificateService certService)
        {
            this.options = options.ThrowIfNull(nameof(options));
            this.certService = certService.ThrowIfNull(nameof(certService));
        }

        /// <summary>
        /// Calculates the key id for a given x509 certificate
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>kid</returns>
        public Task<string> GetKidAsync(X509Certificate2 certificate)
        {
            return Task.FromResult(System.Convert.ToBase64String(certificate.GetCertHash()));
        }

        /// <summary>
        /// Retrieves all public keys that can be used to validate tokens
        /// </summary>
        /// <returns>x509 certificates</returns>
        public Task<IEnumerable<X509Certificate2>> GetPublicKeysAsync()
        {
            return Task.FromResult(certService.GetCertificates());
        }

        /// <summary>
        /// Retrieves the primary signing key
        /// </summary>
        /// <returns>x509 certificate</returns>
        public Task<X509Certificate2> GetSigningKeyAsync()
        {
            certService.GetCertificates();
            return Task.FromResult(options.SigningCertificate);
        }
    }
}
