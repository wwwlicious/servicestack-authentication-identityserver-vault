namespace IdentityServer4.Contrib.Vault.ClientSecretStore.Options
{
    public class VaultClientSecretStoreAppRoleOptions : VaultClientSecretStoreOptions
    {
        public string RoleId { get; set; }

        public string SecretId { get; set; }
    }
}
