// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore
{
    using System.Threading.Tasks;
    using Core.Helpers;
    using Interfaces;
    using Microsoft.IdentityModel.Tokens;
    using Stores;

    public class VaultSigningCredentialStore : ISigningCredentialStore
    {
        private readonly IVaultCertificateService vaultCertificateService;

        public VaultSigningCredentialStore(IVaultCertificateService vaultCertificateService)
        {
            this.vaultCertificateService = vaultCertificateService.ThrowIfNull(nameof(vaultCertificateService));
        }

        public Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            return Task.FromResult(vaultCertificateService.SigningCredentials);
        }
    }
}
