// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault.DTO
{
    using System.Collections.Generic;
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
        [DataMember(Name = "request_id")]
        public string RequestId { get; set; }

        [DataMember(Name = "lease_id")]
        public string LeaseId { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

        [DataMember(Name = "data")]
        public Dictionary<string, string> Data { get; set; }
    }

    //[DataContract]
    //public class SecretData 
    //{
    //    [DataMember(Name = "value")]
    //    public byte[] Value { get; set; }
    //}
}
