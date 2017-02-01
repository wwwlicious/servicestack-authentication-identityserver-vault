// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Interfaces;
    using DTO;
    using Interfaces;
    using Microsoft.Extensions.Logging;

    class VaultSecretStore : IVaultSecretStore
    {
        private readonly IVaultClient vaultClient;
        private readonly ILogger<VaultSecretStore> logger;

        public VaultSecretStore(IVaultClient vaultClient, ILogger<VaultSecretStore> logger)
        {
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
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
                logger.LogError("Unable to decrypt client secret", exception);
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
                    return response.GetSecrets();
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Unable to retrieve scope secrets", exception);
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
