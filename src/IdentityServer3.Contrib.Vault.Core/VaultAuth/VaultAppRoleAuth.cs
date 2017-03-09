// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
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
                            var error = $"Unable to authenticate client using AppRole - Errors: [{string.Join(",", result.Errors)}]";
                            Logger.Error(error);
                            throw new UnauthorizedAccessException(error);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Unable to authenticate client using AppId", exception);
                throw new UnauthorizedAccessException("Unable to authenticate client using AppId", exception);
            }
        }
    }
}
