namespace ServiceStack.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using Interfaces;
    using Logging;

    [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
    public class VaultAppIdAuth : IVaultAuth
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VaultAppIdAuth));

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
