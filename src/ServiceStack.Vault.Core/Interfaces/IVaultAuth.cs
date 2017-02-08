namespace ServiceStack.Vault.Core.Interfaces
{
    public interface IVaultAuth
    {
        void Authenticate(IVaultClientUri vaultUri);

        string AuthToken { get; }
    }
}
