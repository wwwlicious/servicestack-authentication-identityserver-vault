namespace ServiceStack.Vault.Core.VaultAuth
{
    using Interfaces;

    public class VaultTokenAuth : IVaultAuth
    {
        public VaultTokenAuth(string authToken)
        {
            authToken.ThrowIfNullOrEmpty(nameof(authToken));

            AuthToken = authToken;
        }

        public void Authenticate(IVaultClientUri vaultUri)
        {
        }

        public string AuthToken { get; }
    }
}
