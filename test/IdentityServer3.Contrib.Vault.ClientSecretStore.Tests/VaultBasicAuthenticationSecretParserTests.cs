// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Microsoft.Owin;
    using Xunit;

    public class VaultBasicAuthenticationSecretParserTests
    {
        [Fact]
        public void Ctor_VaultSettingsNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new VaultBasicAuthenticationSecretParser(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("secretStore");
        }

        [Fact]
        public void Ctor_ContextParserNull_ThrowsArgumentNullException()
        {
            var vaultSecretStore = A.Fake<IVaultSecretStore>();

            Action ctor = () => new VaultBasicAuthenticationSecretParser(vaultSecretStore, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("contextParser");
        }

        [Fact]
        public void ParseAsync_HeaderNull_ReturnsNull()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();

            A.CallTo(() => parserFake.HeaderDictionary(A<IDictionary<string, object>>._)).Returns(null);

            var parser = new VaultBasicAuthenticationSecretParser(secretStoreFake, parserFake);

            var result = parser.ParseAsync(new Dictionary<string, object>()).Result;

            result.Should().BeNull();
        }

        [Fact]
        public void ParseAsync_AuthorizationHeaderNull_ReturnsNull()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();

            A.CallTo(() => parserFake.HeaderDictionary(A<IDictionary<string, object>>._))
             .Returns(new HeaderDictionary(new Dictionary<string, string[]>()));

            var parser = new VaultBasicAuthenticationSecretParser(secretStoreFake, parserFake);

            var result = parser.ParseAsync(new Dictionary<string, object>()).Result;

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("12345")]
        [InlineData("Basic123456")]
        [InlineData("Basic ")]
        [InlineData("Basic 123456")]
        public void ParseAsync_AuthorizationHeaderInvalid_ReturnsNull(string headerValue)
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();

            A.CallTo(() => parserFake.HeaderDictionary(A<IDictionary<string, object>>._))
             .Returns(new HeaderDictionary(new Dictionary<string, string[]>
             {
                 { "Authorization",  new []{ headerValue } }
             }));

            var parser = new VaultBasicAuthenticationSecretParser(secretStoreFake, parserFake);

            var result = parser.ParseAsync(new Dictionary<string, object>()).Result;
        }

        [Fact]
        public void ParseAsync_DecryptSecret_ReturnsNull()
        {
            var settingsFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();

            A.CallTo(() => parserFake.HeaderDictionary(A<IDictionary<string, object>>._))
             .Returns(new HeaderDictionary(new Dictionary<string, string[]>
             {

             }));
        }
    }
}
