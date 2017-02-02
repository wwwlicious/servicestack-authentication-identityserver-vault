namespace IdentityServer4.Contrib.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    public class VaultAppRoleAuth : IVaultAuth
    {
        private readonly string roleId;
        private readonly string secretId;
        private readonly ILogger<VaultAppRoleAuth> logger;

        public VaultAppRoleAuth(string roleId, string secretId, ILogger<VaultAppRoleAuth> logger)
        {
            this.roleId = roleId.ThrowIfNullOrEmpty(nameof(roleId));
            this.secretId = secretId.ThrowIfNullOrEmpty(nameof(secretId));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public string AuthToken { get; set; }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post<AppRoleLogin, AppRoleLoginResult>($"v1/auth/approle/login", new AppRoleLogin
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
                            logger.LogError("Unable to authenticate client using AppRole - Errors: [{0}]", string.Join(",", result.Errors));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Unable to authenticate client using AppId", exception);
                throw;
            }
        }
    }
}
