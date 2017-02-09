// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Auth
    {
        [DataMember(Name = "client_token")]
        public string ClientToken { get; set; }

        [DataMember(Name = "accessor")]
        public string Accessor { get; set; }

        [DataMember(Name = "policies")]
        public string[] Policies { get; set; }

        [DataMember(Name = "metadata")]
        public Metadata Metadata { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }
    }
}
