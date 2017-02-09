// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core.DTO
{
    using System.Runtime.Serialization;

    [Route("v1/auth/app-id/login/")]
    [DataContract]
    public class AppIdLogin : IUrlFilter, IReturn<AppIdLoginResult>
    {
        [IgnoreDataMember]
        public string AppId { get; set; }

        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        public string ToUrl(string absoluteUrl)
        {
            return absoluteUrl + AppId.TrimStart('/');
        }
    }

    [DataContract]
    public class AppIdLoginResult
    {
        [DataMember(Name = "lease_id")]
        public string LeaseId { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

        [DataMember(Name = "auth")]
        public Auth Auth { get; set; }
    }

    [DataContract]
    public class Metadata
    {
        [DataMember(Name = "app-id")]
        public string AppId { get; set; }

        [DataMember(Name = "user-id")]
        public string UserId { get; set; }
    }
}
