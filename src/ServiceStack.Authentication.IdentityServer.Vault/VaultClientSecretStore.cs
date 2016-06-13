// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using DTO;
    using Configuration;
    using Logging;
    using ServiceStack.Vault.Core.Interfaces;

    public class VaultClientSecretStore : IClientSecretStore
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private static readonly ILog Log = LogManager.GetLogger(typeof(VaultClientSecretStore));

        private readonly IAppSettings appSettings;
        private readonly IVaultClient vaultClient;
        
        public VaultClientSecretStore(IAppSettings appSettings, IVaultClient vaultClient)
        {
            appSettings.ThrowIfNull(nameof(appSettings));
            vaultClient.ThrowIfNull(nameof(vaultClient));

            this.appSettings = appSettings;
            this.vaultClient = vaultClient;
        }

        public async Task<string> GetSecretAsync(string clientId)
        {
            string[] secrets = await GetClientSecrets(clientId).ConfigureAwait(false);
            if (secrets == null) return null;

            var secret = secrets[Random.Next(secrets.Length)];

            return await EncryptString(secret).ConfigureAwait(false);
        }

        private async Task<string[]> GetClientSecrets(string clientId)
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var response = await client.GetAsync(new ReadSecrets { Key = clientId }).ConfigureAwait(false);
                    return response.GetValue<string[]>();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to obtain client secrets from Vault", exception);
                return null;
            }
        }

        private async Task<string> EncryptString(string secret)
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var response = await client.PutAsync(new Encrypt
                    {
                        Key = appSettings.GetString(IdentityServerVaultAuthFeature.VaultEncryptionKeyAppSetting),
                        Value = Base64Encode(secret)
                    }).ConfigureAwait(false);
                    return response?.Data?.CipherText;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to encrypt client secrets from Vault", exception);
                return null;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
