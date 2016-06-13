// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.SecretAppSettings.DTO
{
    using System.Runtime.Serialization;
    using System.Text;

    [Route("v1/secret/", "GET")]
    public class ReadSecrets : IUrlFilter, IReturn<ReadSecretsResponse>
    {
        [IgnoreDataMember]
        public string Key { get; set; }

        public string ToUrl(string absoluteUrl)
        {
            return absoluteUrl + Key.TrimStart('/');
        }
    }

    [DataContract]
    public class ReadSecretsResponse
    {
        [DataMember(Name = "data")]
        public SecretData Data { get; set; }

        public T GetValue<T>()
        {
            return Encoding.UTF8.GetString(Data.Value).FromJson<T>();
        }
    }

    [DataContract]
    public class SecretData
    {
        [DataMember(Name = "value")]
        public byte[] Value { get; set; }
    }
}
