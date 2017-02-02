namespace IdentityServer4.Contrib.Vault.CertificateStore.Options
{
    public class VaultCertificateStoreAppRoleOptions : VaultCertificateStoreOptions
    {
        public string RoleId { get; set; }

        public string SecretId { get; set; }
    }
}
