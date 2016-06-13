// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault.DTO
{
    using System.Runtime.Serialization;

    [Route("v1/transit/encrypt/", "PUT")]
    [DataContract]
    public class Encrypt : IUrlFilter, IReturn<EncryptResponse>
    {
        [IgnoreDataMember]
        public string Key { get; set; }

        [DataMember(Name = "plaintext")]
        public string Value { get; set; }

        public string ToUrl(string absoluteUrl)
        {
            return absoluteUrl + Key.TrimStart('/');
        }
    }

    [DataContract]
    public class EncryptResponse
    {
        [DataMember(Name = "data")]
        public EncryptedData Data { get; set; }
    }

    public class EncryptedData
    {
        [DataMember(Name = "ciphertext")]
        public string CipherText { get; set; }
    }
}
