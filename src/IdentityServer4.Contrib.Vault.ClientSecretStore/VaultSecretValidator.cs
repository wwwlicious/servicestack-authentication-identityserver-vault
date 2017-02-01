// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Microsoft.Extensions.Logging;
    using Models;
    using Validation;

    /// <summary>
    /// Vault Secret Validator
    /// </summary>
    public class VaultSecretValidator : ISecretValidator
    {
        private readonly ILogger<VaultSecretValidator> logger;

        public VaultSecretValidator(ILogger<VaultSecretValidator> logger)
        {
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <summary>Validate Async</summary>
        /// <param name="secrets">List of secrets from Vault</param>
        /// <param name="parsedSecret">Parsed Seccret</param>
        /// <returns></returns>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != Constants.VaultSharedSecretType)
            {
                logger.LogDebug($"Parsed secret should not be of type {parsedSecret.Type}");
                return fail;
            }

            var sharedSecret = parsedSecret.Credential as string;

            if (string.IsNullOrWhiteSpace(sharedSecret))
            {
                logger.LogDebug("Id or Credential is missing");
                return fail;
            }

            foreach (var secret in secrets)
            {
                if (secret.Type != Constants.VaultSharedSecretType)
                {
                    logger.LogDebug($"Skipping secret: {secret.Description}, secret is not of type {Constants.VaultSharedSecretType}");
                    continue;
                }

                if (secret.Value == sharedSecret) return success;
            }

            return fail;
        }
    }
}
