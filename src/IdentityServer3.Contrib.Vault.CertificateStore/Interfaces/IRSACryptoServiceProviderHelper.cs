// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Interfaces
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
