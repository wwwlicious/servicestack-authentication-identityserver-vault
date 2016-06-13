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

    public class VaultAppIdAuth : IVaultAuth
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly string appId;
        private readonly string userId;

        public VaultAppIdAuth(string appId, string userId)
        {
            this.appId = appId.ThrowIfNullOrEmpty(nameof(appId));
            this.userId = userId.ThrowIfNullOrEmpty(nameof(userId));
        }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post<AppIdLogin, AppIdLoginResult>($"v1/auth/app-id/login/{appId}", new AppIdLogin
                    {
                        UserId = userId
                    });

                    if (result?.Auth != null)
                    {
                        AuthToken = result.Auth.ClientToken;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => "Unable to authenticate client using AppId", exception);
                throw;
            }
        }

        public string AuthToken { get; set; }
    }
}
