namespace IdentityServer4.Contrib.Vault.CertificateStore
{
    using System;
    using Core.Helpers;
    using Core.Interfaces;
    using DTO;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    public class VaultCertificateStore : IVaultCertificateStore
    {
        private readonly ILogger logger;

        private readonly IVaultClient vaultClient;

        private readonly string roleName;
        private readonly string commonName;

        public VaultCertificateStore(IVaultClient vaultClient, string roleName, string commonName, ILogger logger)
        {
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
            this.roleName = roleName.ThrowIfNullOrEmpty(nameof(roleName));
            this.commonName = commonName.ThrowIfNullOrEmpty(nameof(commonName));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public VaultCertificate GetCertificate()
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var request = new PkiIssue(commonName);
                    var response = client.Post<PkiIssue, PkiIssueResult>($"v1/pki/issue/{roleName}", request);
                    if (response?.Data == null) return null;
                    return new VaultCertificate
                    {
                        Certificate = response.Data.Certificate,
                        PrivateKey = response.Data.PrivateKey
                    };
                }
            }
            catch (Exception exception)
            {
                logger.LogError($"Unable to get certificate role: {roleName} cn: {commonName}", exception);
                return null;
            }
        }
    }
}
