// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Interfaces;
    using Microsoft.IdentityModel.Tokens;
    using Stores;

    public class VaultValidationKeysStore : IValidationKeysStore
    {
        private readonly IVaultCertificateService vaultCertificateService;

        public VaultValidationKeysStore(IVaultCertificateService vaultCertificateService)
        {
            this.vaultCertificateService = vaultCertificateService.ThrowIfNull(nameof(vaultCertificateService));
        }

        public Task<IEnumerable<SecurityKey>> GetValidationKeysAsync()
        {
            return Task.FromResult(new List<SecurityKey> {  vaultCertificateService.SigningCredentials.Key }.AsEnumerable());
        }
    }
}
