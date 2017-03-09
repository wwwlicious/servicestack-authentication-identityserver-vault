// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Exceptions;
    using Helpers;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
    public class VaultAppIdAuth : IVaultAuth
    {
        private readonly string appId;
        private readonly string userId;
        private readonly ILogger<VaultAppIdAuth> logger;

        public VaultAppIdAuth(string appId, string userId, ILogger<VaultAppIdAuth> logger)
        {
            this.appId = appId.ThrowIfNullOrEmpty(nameof(appId));
            this.userId = userId.ThrowIfNullOrEmpty(nameof(userId));
            this.logger = logger.ThrowIfNull(nameof(logger));
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
                    else
                    {
                        if (result?.Errors != null)
                        {
                            logger.LogError("Unable to authenticate client using AppId - Errors: {0}", result.Errors);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Unable to authenticate client using AppId", exception);
                throw new AuthenticationException("Unable to authenticate client using AppId", exception);
            }
        }

        public string AuthToken { get; set; }
    }
}
