// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;
    using Core.Helpers;
    using Extensions;
    using IdentityModel;
    using Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Models;
    using Validation;

    public class VaultBasicAuthenticationSecretParser : ISecretParser
    {
        private readonly ILogger<VaultBasicAuthenticationSecretParser> logger;

        private readonly IVaultSecretStore secretStore;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>
        /// <param name="logger">Logger</param>
        public VaultBasicAuthenticationSecretParser(
            IVaultSecretStore secretStore, 
            ILogger<VaultBasicAuthenticationSecretParser> logger)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.BasicAuthentication;

        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        { 
            logger.LogDebug("Start parsing Basic Authentication secret");
            var header = context.Request.Headers;
            if (header == null)
            {
                return null;
            }

            if (!header.ContainsKey("Authorization"))
            {
                return null;
            }

            var authorizationHeaders = header["Authorization"];
            if (authorizationHeaders.IsNullOrEmpty())
            {
                return null;
            }

            var authorizationHeader = authorizationHeaders.FirstOrDefault(x => x.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase));
            if (authorizationHeader == null)
            {
                return null;
            }

            var parameter = authorizationHeader.Substring("Basic ".Length);

            string pair;
            try
            {
                pair = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
            }
            catch (FormatException)
            {
                logger.LogDebug("Malformed Basic Authentication credential.");
                return null;
            }
            catch (ArgumentException)
            {
                logger.LogDebug("Malformed Basic Authentication credential.");
                return null;
            }

            var ix = pair.IndexOf(':');
            if (ix == -1)
            {
                logger.LogDebug("Malformed Basic Authentication credential.");
                return null;
            }

            var clientId = pair.Substring(0, ix);
            var secret = pair.Substring(ix + 1);

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
            {
                return null;
            }

            // Decrypt secrypt from Vault
            var decryptedSecret = await secretStore.GetDecryptedSecret(clientId, secret).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(decryptedSecret))
            {
                return null;
            }

            return new ParsedSecret
            {
                Id = clientId,
                Credential = decryptedSecret,
                Type = Constants.VaultSharedSecretType
            };
        }
    }
}
