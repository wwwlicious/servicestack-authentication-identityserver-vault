// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.SecretAppSettings.DTO
{
    using System.Runtime.Serialization;

    [Route("v1/secret/")]
    [DataContract]
    class WriteSecret : IUrlFilter
    {
        [IgnoreDataMember]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        public byte[] Value { get; set; }

        public string ToUrl(string absoluteUrl)
        {
            return absoluteUrl + Key.Trim('/');
        }
    }
}
