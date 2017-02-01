// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    public class VaultUserPassAuth : IVaultAuth
    {
        private readonly string username;
        private readonly string password;

        private readonly ILogger<VaultUserPassAuth> logger;

        public VaultUserPassAuth(string username, string password, ILogger<VaultUserPassAuth> logger)
        {
            this.username = username.ThrowIfNullOrEmpty(nameof(username));
            this.password = password.ThrowIfNullOrEmpty(nameof(password));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post<UserPassLogin, UserPassLoginResult>($"v1/userpass/login/{username}", new UserPassLogin
                    {
                        Password = password
                    });

                    AuthToken = result.Auth.ClientToken;
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Unable to authenticate client using AppId", exception);
                throw;
            }
        }

        public string AuthToken { get; private set; }
    }
}
