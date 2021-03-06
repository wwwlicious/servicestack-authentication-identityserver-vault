﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Interfaces;
    using Logging;

    public class VaultUserPassAuth : IVaultAuth
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VaultUserPassAuth));

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
                    var result = client.Post(new UserPassLogin
                    {
                        Username = username,
                        Password = password
                    });

                    AuthToken = result.Auth.ClientToken;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to authenticate client using AppId", exception);
                throw;
            }
        }

        public string AuthToken { get; private set; }
    }
}
