namespace IdentityServer3.Contrib.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using IdentityServer3.Core.Logging;
    using Interfaces;

    public class VaultAppRoleAuth : IVaultAuth
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly string roleId;
        private readonly string secretId;

        public VaultAppRoleAuth(string roleId, string secretId)
        {
            this.roleId = roleId.ThrowIfNullOrEmpty(nameof(roleId));
            this.secretId = secretId.ThrowIfNullOrEmpty(nameof(secretId));
        }

        public string AuthToken { get; set; }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post<AppRoleLogin, AppRoleLoginResult>("v1/auth/approle/login", new AppRoleLogin
                    {
                        RoleId = roleId,
                        SecretId = secretId
                    });

                    if (result?.Auth != null)
                    {
                        AuthToken = result.Auth.ClientToken;
                    }
                    else
                    {
                        if (result?.Errors != null)
                        {
                            Logger.ErrorFormat("Unable to authenticate client using AppRole - Errors: [{0}]", string.Join(",", result.Errors));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Unable to authenticate client using AppId", exception);
                throw;
            }
        }
    }
}
