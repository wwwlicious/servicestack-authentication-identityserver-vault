// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore
{
    using System.Threading.Tasks;
    using Core.Helpers;
    using IdentityModel;
    using Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Models;
    using Validation;

    /// <summary>
    /// Post Body Secret Parser
    /// </summary>
    public class VaultPostBodySecretParser : ISecretParser
    {
        private readonly IVaultSecretStore secretStore;
        private readonly ILogger<VaultPostBodySecretParser> logger;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>        
        /// <param name="logger"></param>
        public VaultPostBodySecretParser(IVaultSecretStore secretStore, ILogger<VaultPostBodySecretParser> logger)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.PostBody;

        /// <summary>Parse Secret from form post</summary>
        /// <param name="context">HTTP Context</param>
        /// <returns></returns>
        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            logger.LogDebug("Start parsing for secret in post body");

            var body = context.Request.Form;
            if (body == null)
            {
                return null;
            }

            var id = body["client_id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var secret = body["client_secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                return null;
            }

            var decryptedSecret = await secretStore.GetDecryptedSecret(id, secret).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(decryptedSecret))
            {
                return null;
            }

            return new ParsedSecret
            {
                Id = id,
                Credential = decryptedSecret,
                Type = Constants.VaultSharedSecretType
            };
        }
    }
}
