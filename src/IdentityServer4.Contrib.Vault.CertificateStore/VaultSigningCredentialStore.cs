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
