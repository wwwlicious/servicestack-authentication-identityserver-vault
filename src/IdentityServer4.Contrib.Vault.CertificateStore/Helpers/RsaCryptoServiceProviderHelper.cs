namespace IdentityServer4.Contrib.Vault.CertificateStore.Helpers
{
    using System;
    using System.Security.Cryptography;
    using Interfaces;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Pkcs;
    using Org.BouncyCastle.Math;

    public class RsaCryptoServiceProviderHelper : IRSACryptoServiceProviderHelper
    {
        /// <summary>Get the Private Key Provider from the Vault Private Key</summary>
        /// <param name="privateKeyData">Private Key Data</param>
        /// <returns>RSA Crypto Service Provider</returns>
        public RSACryptoServiceProvider GetPrivateKeyProvider(byte[] privateKeyData)
        {
            var stream = new Asn1InputStream(privateKeyData).ReadObject();
            var rsaKey = RsaPrivateKeyStructure.GetInstance(stream);

            var rsaParameters = new RSAParameters
            {
                Modulus = rsaKey.Modulus.ToByteArrayUnsigned(),
                Exponent = rsaKey.PublicExponent.ToByteArrayUnsigned(),
                P = rsaKey.Prime1.ToByteArrayUnsigned(),
                Q = rsaKey.Prime2.ToByteArrayUnsigned()                
            };

            rsaParameters.D = ConvertRsaParametersField(rsaKey.PrivateExponent, rsaParameters.Modulus.Length);
            rsaParameters.DP = ConvertRsaParametersField(rsaKey.Exponent1, rsaParameters.P.Length);
            rsaParameters.DQ = ConvertRsaParametersField(rsaKey.Exponent2, rsaParameters.Q.Length);
            rsaParameters.InverseQ = ConvertRsaParametersField(rsaKey.Coefficient, rsaParameters.Q.Length);

            var cspParameters = new CspParameters
            {
                KeyContainerName = Constants.PrivateKeyContainerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var rsaProvider = new RSACryptoServiceProvider(cspParameters);
            rsaProvider.ImportParameters(rsaParameters);

            return rsaProvider;
        }

        private static byte[] ConvertRsaParametersField(BigInteger n, int size)
        {
            byte[] bs = n.ToByteArrayUnsigned();

            if (bs.Length == size)
                return bs;

            if (bs.Length > size)
                throw new ArgumentException("Specified size too small", nameof(size));

            byte[] padded = new byte[size];
            Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
            return padded;
        }
    }
}
