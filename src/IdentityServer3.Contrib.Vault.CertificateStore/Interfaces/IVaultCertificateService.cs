
namespace IdentityServer3.Contrib.Vault.CertificateStore.Interfaces
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    public interface IVaultCertificateService
    {
        IEnumerable<X509Certificate2> GetCertificates();
    }
}
