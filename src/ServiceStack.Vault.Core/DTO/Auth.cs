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
