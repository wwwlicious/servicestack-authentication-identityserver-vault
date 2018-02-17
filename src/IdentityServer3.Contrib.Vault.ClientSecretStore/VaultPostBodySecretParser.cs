// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers;
    using IdentityServer3.Core.Logging;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using Interfaces;

    /// <summary>
    /// Post Body Secret Parser
    /// </summary>
    public class VaultPostBodySecretParser : ISecretParser
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IVaultSecretStore secretStore;
        private readonly IRequestParser parser;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>
        /// <param name="parser">Owin Context Form Collection Parser</param>
        public VaultPostBodySecretParser(IVaultSecretStore secretStore, IRequestParser parser)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.parser = parser.ThrowIfNull(nameof(parser));
        }

        /// <summary>Parse Secret from form post</summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public async Task<ParsedSecret> ParseAsync(IDictionary<string, object> environment)
        {
            Logger.Debug("Start parsing for secret in post body");

            var body = await parser.ReadRequestFormAsync(environment).ConfigureAwait(false);

            var id = body?.Get("client_id");
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var secret = body.Get("client_secret");
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
