// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Interfaces;
    using DTO;
    using Helpers;
    using IdentityServer3.Core.Logging;
    using Interfaces;

    class VaultSecretStore : IVaultSecretStore
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IVaultClient vaultClient;

        public VaultSecretStore(IVaultClient vaultClient)
        {
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
        }

        public async Task<string> GetDecryptedSecret(string id, string secret)
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var response = await client.PostAsync<DecryptSecret, DecryptSecretResponse>($"v1/transit/decrypt/{id}", 
                        new DecryptSecret
                        {
                            CipherText = secret
                        }).ConfigureAwait(false);
                    if (response?.Data == null)
                    {
                        return null;
                    }
                    return Base64Decode(response.Data.PlainText);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => "Unable to decrypt client secret", exception);
            }

            return null;
        }

        public async Task<string[]> GetSecrets(string secretName)
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var response = await client.GetAsync<SecretsResponse>($"v1/secret/{secretName}")
                                               .ConfigureAwait(false);
                    return response.Data.Select(x => x.Value).ToArray();
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => "Unable to retrieve scope secrets", exception);
                return new string[0];
            }
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
