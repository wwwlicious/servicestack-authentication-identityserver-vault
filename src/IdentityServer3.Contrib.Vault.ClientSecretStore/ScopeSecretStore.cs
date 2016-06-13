// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using Interfaces;

    /// <summary>
    /// Scope Secret Store - Appends secrets from vault to a scope object
    /// </summary>
    public class ScopeSecretStore : IScopeStore
    {
        private readonly IVaultSecretStore secretStore;
        private readonly IScopeStore scopeStore;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>
        /// <param name="scopeStore">Scope Store</param>
        public ScopeSecretStore(IVaultSecretStore secretStore, IScopeStore scopeStore)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.scopeStore = scopeStore.ThrowIfNull(nameof(scopeStore));
        }

        /// <summary>Find Scopes by Name</summary>
        /// <param name="scopeNames">Scope Names</param>
        /// <returns>Scope objects</returns>
        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                return new List<Scope>();
            }

            var names = scopeNames.ToList();
            if (names.Count == 0)
            {
                return new List<Scope>();
            }

            var scopeResult = await scopeStore.FindScopesAsync(names).ConfigureAwait(false);

            if (scopeResult == null) return new List<Scope>();

            var scopes = scopeResult.ToList();
            if (scopes.Count == 0) return new List<Scope>();
            
            foreach (var scope in scopes)
            {
                // Ignore standard scopes
                if (StandardScopes.All.Any(x => x.Name == scope.Name)) continue;
                if (StandardScopes.OfflineAccess.Name == scope.Name) continue;

                var scopeSecrets = await secretStore.GetSecrets(scope.Name).ConfigureAwait(false);
                scope.ScopeSecrets = scopeSecrets.Select(x => new Secret(x)
                {
                    Type = Constants.VaultSharedSecretType
                }).ToList();
            }

            return scopes;
        }

        /// <summary>
        /// Get Scope Secrets
        /// </summary>
        /// <param name="publicOnly">Public Only Flag</param>
        /// <returns>List of Scopes</returns>
        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            return scopeStore.GetScopesAsync(publicOnly);
        }
    }
}
