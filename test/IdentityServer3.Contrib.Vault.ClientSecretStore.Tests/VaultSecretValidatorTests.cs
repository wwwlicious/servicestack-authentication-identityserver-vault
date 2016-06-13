// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests
{
    using System.Collections.Generic;
    using Core.Models;
    using FluentAssertions;
    using Xunit;

    public class VaultSecretValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        public void ValidateAsync_ParsedSecretNotSharedSecretType_ReturnsFail(string parsedSecretType)
        {
            var parsedSecret = new ParsedSecret { Type = parsedSecretType };

            var validator = new VaultSecretValidator();

            // Act
            var result = validator.ValidateAsync(new List<Secret>(), parsedSecret).Result;

            result.Success.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ValidateAsync_ParsedSecretCredentialNullOrEmpty_ReturnsFail(string credential)
        {
            var parsedSecret = new ParsedSecret { Type = Constants.VaultSharedSecretType, Credential = credential };

            var validator = new VaultSecretValidator();

            // Act
            var result = validator.ValidateAsync(new List<Secret>(), parsedSecret).Result;

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void ValidateAsync_SecretsNotOfValidType_ReturnsFail()
        {
            var validator = new VaultSecretValidator();

            // Act
            var result = validator.ValidateAsync(new List<Secret>
            {
                new Secret { Type = null },
                new Secret { Type = "" },
                new Secret { Type = "abc" }
            }, new ParsedSecret { Credential = "1234", Type = Constants.VaultSharedSecretType }).Result;

            result.Success.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("secret56789")]
        public void ValidateAsync_SecretsNotMatching_ReturnFail(string credential)
        {
            var validator = new VaultSecretValidator();

            // Act
            var result = validator.ValidateAsync(new List<Secret>
            {
                new Secret { Type = Constants.VaultSharedSecretType, Value = credential }
            }, new ParsedSecret { Credential = "secret1234", Type = Constants.VaultSharedSecretType }).Result;

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void ValidateAsync_Matches_ReturnsSuccess()
        {
            var validatr = new VaultSecretValidator();

            // Act
            var result = validatr.ValidateAsync(new List<Secret>
            {
                new Secret { Type = Constants.VaultSharedSecretType, Value = "secret1234" }
            }, new ParsedSecret { Credential = "secret1234", Type = Constants.VaultSharedSecretType }).Result;

            result.Success.Should().BeTrue();
        }
    }
}
