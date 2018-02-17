// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using IdentityServer3.Core.Logging;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using Interfaces;

    /// <summary>
    /// Basic Authentication Secret Parser
    /// </summary>
    public class VaultBasicAuthenticationSecretParser : ISecretParser
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IVaultSecretStore secretStore;
        private readonly IRequestParser contextParser;

        /// <summary>Constructor</summary>
        /// <param name="secretStore">Vault Secret Store</param>
        /// <param name="contextParser">Owin Context Parser</param>
        public VaultBasicAuthenticationSecretParser(IVaultSecretStore secretStore, IRequestParser contextParser)
        {
            this.secretStore = secretStore.ThrowIfNull(nameof(secretStore));
            this.contextParser = contextParser.ThrowIfNull(nameof(contextParser));
        }

        /// <summary>
        /// Parse Basic Authentication from Request Header
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>Parsed Secret</returns>
        public async Task<ParsedSecret> ParseAsync(IDictionary<string, object> environment)
        {
            Logger.Debug("Start parsing Basic Authentication secret");

            var header = contextParser.HeaderDictionary(environment);

            var authorizationHeader = header?.Get("Authorization");
            if (authorizationHeader == null)
            {
                return null;
            }

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
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
                Logger.Debug("Malformed Basic Authentication credential.");
                return null;
            }
            catch (ArgumentException)
            {
                Logger.Debug("Malformed Basic Authentication credential.");
                return null;
            }

            var ix = pair.IndexOf(':');
            if (ix == -1)
            {
                Logger.Debug("Malformed Basic Authentication credential.");
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
