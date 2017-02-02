namespace IdentityServer4.Contrib.Vault.CertificateStore.Options
{
    using System.Security.Cryptography.X509Certificates;

    public abstract class VaultCertificateStoreOptions
    {
        protected VaultCertificateStoreOptions()
        {
            VaultUrl = "http://127.0.0.1:8200";
        }

        public string VaultUrl { get; set; }

        public X509Certificate2 VaultCertificate { get; set; }

        public string RoleName { get; set; }

        public string CommonName { get; set; }
    }
}
