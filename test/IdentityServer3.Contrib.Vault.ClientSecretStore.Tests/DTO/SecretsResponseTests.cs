// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests.DTO
{
    using System.Text;
    using FluentAssertions;
    using Vault.ClientSecretStore.DTO;
    using Xunit;

    public class SecretsResponseTests
    {
        [Theory]
        [InlineData("[\"secret1\",\"secret2\",\"secret3\",\"secret4\",\"secret5\"]")]
        public void GetSecrets_ValidData(string secretString)
        {
            var secretsResponse = new SecretsResponse
            {
                Data = new SecretData {Value = Encoding.UTF8.GetBytes(secretString) }
            };

            var secrets = secretsResponse.GetSecrets();

            secrets.Length.Should().Be(5);
            secrets.Should().BeEquivalentTo("secret1", "secret2", "secret3", "secret4", "secret5");
        }
    }
}
