namespace IdentityServer4.Contrib.Vault.CertificateStore.Interfaces
{
    using System.Security.Cryptography;

    public interface IRSACryptoServiceProviderHelper
    {
        /// <summary>Get the Private Key Provider from the Vault Private Key</summary>
        /// <param name="privateKeyData">Private Key Data</param>
        /// <returns>RSA Crypto Service Provider</returns>
        RSACryptoServiceProvider GetPrivateKeyProvider(byte[] privateKeyData);
    }
}
