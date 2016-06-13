// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityServer3.Core.Logging;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Validation;

    /// <summary>
    /// Vault Secret Validator
    /// </summary>
    public class VaultSecretValidator : ISecretValidator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        /// <summary>Validate Async</summary>
        /// <param name="secrets">List of secrets from Vault</param>
        /// <param name="parsedSecret">Parsed Seccret</param>
        /// <returns></returns>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult {Success = false});
            var success = Task.FromResult(new SecretValidationResult {Success = true});

            if (parsedSecret.Type != Constants.VaultSharedSecretType)
            {
                Logger.Debug($"Parsed secret should not be of type {parsedSecret.Type}");
                return fail;
            }

            var sharedSecret = parsedSecret.Credential as string;

            if (string.IsNullOrWhiteSpace(sharedSecret))
            {                
                Logger.Debug("Id or Credential is missing");
                return fail;
            }

            foreach (var secret in secrets)
            {
                if (secret.Type != Constants.VaultSharedSecretType)
                {
                    Logger.Debug($"Skipping secret: {secret.Description}, secret is not of type {Constants.VaultSharedSecretType}");
                    continue;
                }

                if (secret.Value == sharedSecret) return success;
            }

            return fail;
        }
    }
}