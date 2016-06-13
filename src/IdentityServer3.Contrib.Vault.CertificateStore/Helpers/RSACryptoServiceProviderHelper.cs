// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Helpers
{
    using System.Security.Cryptography;
    using Interfaces;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Pkcs;
    using Org.BouncyCastle.Security;

    class RsaCryptoServiceProviderHelper : IRSACryptoServiceProviderHelper
    {
        /// <summary>Get the Private Key Provider from the Vault Private Key</summary>
        /// <param name="privateKeyData">Private Key Data</param>
        /// <returns>RSA Crypto Service Provider</returns>
        public RSACryptoServiceProvider GetPrivateKeyProvider(byte[] privateKeyData)
        {
            var stream = new Asn1InputStream(privateKeyData).ReadObject();
            var privateKeyStructure = RsaPrivateKeyStructure.GetInstance(stream);
            var rsaParameters = DotNetUtilities.ToRSAParameters(privateKeyStructure);

            var cspParameters = new CspParameters
            {
                KeyContainerName = Constants.PrivateKeyContainerName
            };
            var rsaProvider = new RSACryptoServiceProvider(cspParameters);
            rsaProvider.ImportParameters(rsaParameters);

            return rsaProvider;
        }
    }
}
