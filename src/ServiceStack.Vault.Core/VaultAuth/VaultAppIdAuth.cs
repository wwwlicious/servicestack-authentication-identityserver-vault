// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using Interfaces;
    using Logging;

    public class VaultAppIdAuth : IVaultAuth
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (VaultAppIdAuth));

        private readonly string appId;
        private readonly string userId;

        public VaultAppIdAuth(string appId, string userId)
        {
            this.appId = appId.ThrowIfNullOrWhitespace(nameof(appId));
            this.userId = userId.ThrowIfNullOrWhitespace(nameof(userId));
        }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post(new AppIdLogin
                    {
                        AppId = appId,
                        UserId = userId
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

        public string AuthToken { get; set; }
    }
}
