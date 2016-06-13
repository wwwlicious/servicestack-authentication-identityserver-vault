// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core.DTO
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserPassLogin
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class UserPassLoginResult
    {
        [JsonProperty("lease_id")]
        public string LeaseId { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("auth")]
        public UserPassAuth Auth { get; set; }
    }

    public class UserPassAuth
    {
        [JsonProperty("client_token")]
        public string ClientToken { get; set; }

        [JsonProperty("policies")]
        public string[] Policies { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }
    }
}
