namespace ServiceStack.Vault.Core.Interfaces
{
    public interface IVaultClient
    {
        IVaultClientUri VaultUri { get; }

        IVaultAuth VaultAuth { get; }

        IJsonServiceClient ServiceClient { get; }
    }
}
