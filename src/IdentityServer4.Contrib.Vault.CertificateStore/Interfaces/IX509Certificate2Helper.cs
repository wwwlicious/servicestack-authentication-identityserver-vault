namespace IdentityServer4.Contrib.Vault.CertificateStore.Interfaces
{
    using System.Security.Cryptography.X509Certificates;

    public interface IX509Certificate2Helper
    {
        X509Certificate2 CreateCertificate(byte[] certificateData);
    }
}
