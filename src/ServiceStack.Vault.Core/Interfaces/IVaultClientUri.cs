namespace ServiceStack.Vault.Core.Interfaces
{
    public interface IVaultClientUri
    {
        string VaultUri { get; }

        IJsonServiceClient ServiceClient { get; }
    }
}
