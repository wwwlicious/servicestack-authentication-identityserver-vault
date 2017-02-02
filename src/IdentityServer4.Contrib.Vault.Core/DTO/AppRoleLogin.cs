namespace IdentityServer4.Contrib.Vault.Core.DTO
{
    using Newtonsoft.Json;

    public class AppRoleLogin
    {
        [JsonProperty("role_id")]
        public string RoleId { get; set; }

        [JsonProperty("secret_id")]
        public string SecretId { get; set; }
    }

    public class AppRoleLoginResult
    {
        [JsonProperty("auth")]
        public Auth Auth { get; set; }

        [JsonProperty("warnings")]
        public string Warnings { get; set; }

        [JsonProperty("wrap_info")]
        public string WrapInfo { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("lease_id")]
        public string LeaseId { get; set; }

        [JsonProperty("errors")]
        public string[] Errors { get; set; }
    }
}
