namespace IdentityServer4.Contrib.Vault.CertificateStore.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using Core.Helpers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Pkcs;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.X509;

    public class X509Certificate2Helper : IX509Certificate2Helper
    {
        private readonly ILogger<X509Certificate2Helper> logger;

        public X509Certificate2Helper(ILogger<X509Certificate2Helper> logger)
        {
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public X509Certificate2 CreateCertificate(byte[] certificateData, byte[] privateKeyData)
        {
            var file = Path.Combine(Path.GetTempPath(), "Idsvr" + Guid.NewGuid());
            WriteTemporaryCertificateFile(file, certificateData, privateKeyData);

            X509Certificate2 certificate;
            try
            {
                certificate = new X509Certificate2(file, string.Empty, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            }
            catch (Exception exception)
            {
                logger.LogError($"Unable to read certificate data from {file}", exception);
                throw;
            }
            finally
            {
                logger.LogDebug($"Cleaning up temporary file at {file}");
                File.Delete(file);
            }
            return certificate;
        }

        private void WriteTemporaryCertificateFile(string fileName, byte[] certificateData, byte[] privateKeyData)
        {
            var certificateEntry = GetCertificateEntry(certificateData);
            var rsaKeyEntry = GetPrivateKeyEntry(privateKeyData);

            var certStore = new Pkcs12Store();
            certStore.SetCertificateEntry("IdentityServer", certificateEntry);
            certStore.SetKeyEntry("IdentityServerKey", rsaKeyEntry, new[] {certificateEntry});

            byte[] pfxBytes;
            using (var memoryStream = new MemoryStream())
            {
                certStore.Save(memoryStream, string.Empty.ToCharArray(), new SecureRandom());
                pfxBytes = memoryStream.ToArray();
            }

            pfxBytes = Pkcs12Utilities.ConvertToDefiniteLength(pfxBytes, string.Empty.ToCharArray());

            try
            {
                File.WriteAllBytes(fileName, pfxBytes);
            }
            catch (Exception exception)
            {
                logger.LogError($"Unable to create certificate from {fileName}", exception);
                throw;
            }
        }

        private X509CertificateEntry GetCertificateEntry(byte[] certificateData)
        {
            var certificate = new X509CertificateParser().ReadCertificate(certificateData);
            return new X509CertificateEntry(certificate);
        }

        private AsymmetricKeyEntry GetPrivateKeyEntry(byte[] privateKeyData)
        {
            var stream = new Asn1InputStream(privateKeyData).ReadObject();
            var rsaKey = RsaPrivateKeyStructure.GetInstance(stream);

            var rsaKeyParameters = new RsaPrivateCrtKeyParameters(
                rsaKey.Modulus,
                rsaKey.PublicExponent,
                rsaKey.PrivateExponent,
                rsaKey.Prime1,
                rsaKey.Prime2,
                rsaKey.Exponent1,
                rsaKey.Exponent2,
                rsaKey.Coefficient);

            return new AsymmetricKeyEntry(rsaKeyParameters);
        }
    }
}