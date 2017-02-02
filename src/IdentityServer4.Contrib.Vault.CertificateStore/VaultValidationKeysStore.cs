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
