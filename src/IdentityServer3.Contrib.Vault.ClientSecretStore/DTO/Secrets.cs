// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.DTO
{
    using System.Text;
    using Newtonsoft.Json;

    public class SecretsResponse
    {
        [JsonProperty("data")]
        public SecretData Data { get; set; }

        public string[] GetSecrets()
        {
            if (Data?.Value == null)
            {
                return new string[0];
            }
            var secrets = Encoding.UTF8.GetString(Data.Value);
            return JsonConvert.DeserializeObject<string[]>(secrets);
        }
    }

    public class SecretData
    {
        [JsonProperty("value")]
        public byte[] Value { get; set; }
    }
}
