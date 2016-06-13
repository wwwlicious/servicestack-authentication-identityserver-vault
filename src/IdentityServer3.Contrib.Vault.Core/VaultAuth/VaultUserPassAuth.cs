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

    public class VaultUserPassAuth : IVaultAuth
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly string username;
        private readonly string password;

        public VaultUserPassAuth(string username, string password)
        {
            username.ThrowIfNullOrEmpty(nameof(username));
            password.ThrowIfNullOrEmpty(nameof(password));

            this.username = username;
            this.password = password;
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
                Logger.Log(LogLevel.Error, () => "Unable to authenticate client using AppId", exception);
                throw;
            }
        }

        public string AuthToken { get; private set; }
    }
}
