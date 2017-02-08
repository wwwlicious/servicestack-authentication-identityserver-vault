namespace ServiceStack.Vault.Core.DTO
{
    using System.Runtime.Serialization;

    [Route("v1/auth/approle/login")]
    [DataContract]
    public class AppRoleLogin : IReturn<AppRoleLoginResult>
    {
        [DataMember(Name = "role_id")]
        public string RoleId { get; set; }

        [DataMember(Name = "secret_id")]
        public string SecretId { get; set; }
    }

    [DataContract]
    public class AppRoleLoginResult
    {
        [DataMember(Name = "auth")]
        public Auth Auth { get; set; }

        [DataMember(Name = "warnings")]
        public string Warnings { get; set; }

        [DataMember(Name = "wrap_info")]
        public string WrapInfo { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

        [DataMember(Name = "lease_id")]
        public string LeaseId { get; set; }

        [DataMember(Name = "errors")]
        public string[] Errors { get; set; }
    }
}
