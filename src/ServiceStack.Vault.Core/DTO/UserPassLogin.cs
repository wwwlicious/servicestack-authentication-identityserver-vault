// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Route("v1/userpass/login/")]
    [DataContract]
    public class UserPassLogin : IUrlFilter, IReturn<UserPassLoginResult>
    {
        [IgnoreDataMember]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        public string ToUrl(string absoluteUrl)
        {
            return absoluteUrl + Username.Trim('/');
        }
    }

    [DataContract]
    public class UserPassLoginResult
    {
        [DataMember(Name = "lease_id")]
        public string LeaseId { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

        [DataMember(Name = "auth")]
        public UserPassAuth Auth { get; set; }
    }

    [DataContract]
    public class UserPassAuth
    {
        [DataMember(Name = "client_token")]
        public string ClientToken { get; set; }

        [DataMember(Name = "policies")]
        public string[] Policies { get; set; }

        [DataMember(Name = "metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }
    }
}
