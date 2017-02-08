namespace ServiceStack.Authentication.IdentityServer.Vault
{
    public class ConfigKeys
    {
        public const string VaultUri = "vault.uri";

        public const string VaultAppId = "vault.app-id";
        public const string VaultUserId = "vault.user-id";
        public const string VaultUserPassword = "vault.user-password";

        public const string VaultAuthMethod = "vault.auth-method";

        public const string VaultAppRoleId = "vault.app-role-id";
        public const string VaultAppRoleSecretId = "vault.app-secret-id";

        public const string VaultEncryptionKey = "vault.encryption.key";        
    }
}
