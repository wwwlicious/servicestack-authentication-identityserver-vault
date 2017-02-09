// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Interfaces;
    using Models;
    using Stores;

    public class ResourceSecretStore : IResourceDataStore
    {
        private readonly IVaultSecretStore secretStore;
        private readonly IResourceStore resourceStore;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>
        /// <param name="resourceStore">Resource Store</param>
        public ResourceSecretStore(IVaultSecretStore secretStore, IResourceStore resourceStore)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.resourceStore = resourceStore.ThrowIfNull(nameof(resourceStore));
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return resourceStore.FindEnabledIdentityResourcesByScopeAsync(scopeNames);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                return new List<ApiResource>();
            }

            var names = scopeNames.ToList();
            if (names.Count == 0)
            {
                return new List<ApiResource>();
            }

            var apiResourceResult = await resourceStore.FindApiResourcesByScopeAsync(names).ConfigureAwait(false);
            if (apiResourceResult == null) return new List<ApiResource>();

            var apiResources = apiResourceResult.ToList();
            if (apiResources.Count == 0) return new List<ApiResource>();

            foreach (var apiResource in apiResources)
            {
                var scopeSecrets = await secretStore.GetSecrets(apiResource.Name).ConfigureAwait(false);
                foreach (var secret in scopeSecrets.Select(x => new Secret(x) { Type = Constants.VaultSharedSecretType }))
                {
                    apiResource.ApiSecrets.Add(secret);
                }
            }
            return apiResources;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var apiResourceResult = await resourceStore.FindApiResourceAsync(name).ConfigureAwait(false);
            if (apiResourceResult == null) return null;

            var scopeSecrets = await secretStore.GetSecrets(apiResourceResult.Name).ConfigureAwait(false);
            foreach (var secret in scopeSecrets.Select(x => new Secret(x) { Type = Constants.VaultSharedSecretType }))
            {
                apiResourceResult.ApiSecrets.Add(secret);
            }

            return apiResourceResult;
        }

        public Task<Resources> GetAllResources()
        {
            return resourceStore.GetAllResources();
        }
    }
}
