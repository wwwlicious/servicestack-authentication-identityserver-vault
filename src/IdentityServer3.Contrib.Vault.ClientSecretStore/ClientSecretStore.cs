// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using Interfaces;

    /// <summary>
    /// Client Secret Store - Appends secrets from vault to a client object
    /// </summary>
    public class ClientSecretStore : IClientStore
    {
        private readonly IVaultSecretStore secretStore;
        private readonly IClientStore clientStore;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Secret Store</param>
        /// <param name="clientStore">Client Store</param>
        public ClientSecretStore(IVaultSecretStore secretStore, IClientDataStore clientStore)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.clientStore = clientStore.ThrowIfNull(nameof(clientStore));
        }

        /// <summary>Find client by Id</summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>Client Object</returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var findClientTask = clientStore.FindClientByIdAsync(clientId);

            var client = await findClientTask.ConfigureAwait(false);

            if (client == null) return null;

            var clientSecrets = new List<Secret>(client.ClientSecrets);
            
            var secrets = secretStore.GetSecrets(clientId).Result;
            if (secrets != null && secrets.Length > 0)
            {
                foreach (var secret in secrets)
                {
                    clientSecrets.Add(new Secret(secret)
                    {
                        Type = Constants.VaultSharedSecretType
                    });
                }
            }

            client.ClientSecrets = clientSecrets;

            return client;
        }
    }
}
