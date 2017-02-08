namespace ServiceStack.Authentication.IdentityServer.Vault.Interfaces
{
    public interface IIdentityServerVaultAuthSettings
    {
        string VaultUri { get; set; }

        string VaultAppId { get; set; }

        string VaultUserId { get; set; }

        string VaultEncryptionKey { get; set; }
    }
}
