// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.Core.DTO
{
    using System;
    using Newtonsoft.Json;

    [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
    public class AppIdLogin
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
    public class AppIdLoginResult
    {
        [JsonProperty("lease_id")]
        public string LeaseId { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("auth")]
        public Auth Auth { get; set; }

        [JsonProperty("errors")]
        public string[] Errors { get; set; }
    }
}
