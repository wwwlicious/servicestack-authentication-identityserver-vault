namespace IdentityServer4.Contrib.Vault.CertificateStore.DTO
{
    using Newtonsoft.Json;

    public class PkiIssue
    {
        public PkiIssue(string commonName)
        {
            CommonName = commonName;
            Format = "der";
        }

        [JsonProperty("common_name")]
        public string CommonName { get; }

        [JsonProperty("alt_names")]
        public string AltNames { get; set; }

        [JsonProperty("ip_sans")]
        public string IpSubjectAlternativeNames { get; set; }

        [JsonProperty("ttl")]
        public string TimeToLive { get; set; }

        [JsonProperty("format")]
        public string Format { get; }
    }

    public class PkiIssueResult
    {
        [JsonProperty("lease_id")]
        public string LeaseId { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("data")]
        public PkiData Data { get; set; }

        [JsonProperty("warnings")]
        public string Warnings { get; set; }

        [JsonProperty("errors")]
        public string[] Errors { get; set; }
    }

    public class PkiData
    {
        [JsonProperty("certificate")]
        public byte[] Certificate { get; set; }

        [JsonProperty("issuing_ca")]
        public byte[] IssuingCa { get; set; }

        [JsonProperty("private_key")]
        public byte[] PrivateKey { get; set; }

        [JsonProperty("private_key_type")]
        public string PrivateKeyType { get; set; }

        [JsonProperty("serial_number")]
        public string SerialNumber { get; set; }
    }
}
