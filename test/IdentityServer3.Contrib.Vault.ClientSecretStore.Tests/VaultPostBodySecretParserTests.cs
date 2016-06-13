// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Microsoft.Owin;
    using Xunit;

    public class VaultPostBodySecretParserTests
    {
        [Fact]
        public void Ctor_VaultSettingsNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new VaultPostBodySecretParser(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("secretStore");
        }

        [Fact]
        public void Ctor_OwinContextParserNull_ThrowsArgumentNullException()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();

            Action ctor = () => new VaultPostBodySecretParser(secretStoreFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("parser");
        }

        [Fact]
        public void ParseAsync_ReturnNullIfBodyNull()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();

            A.CallTo(() => parserFake.ReadRequestFormAsync(A<IDictionary<string, object>>._))
             .Returns(Task.FromResult((IFormCollection)null));

            var parser = new VaultPostBodySecretParser(secretStoreFake, parserFake);

            var parseResult = parser.ParseAsync(new Dictionary<string, object>()).Result;

            parseResult.Should().BeNull();
        }

        [Fact]
        public void ParseAsync_ReturnNullIfClientIdNull()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();
            var formCollectionFake = A.Fake<IFormCollection>();

            A.CallTo(() => parserFake.ReadRequestFormAsync(A<IDictionary<string, object>>._))
             .Returns(Task.FromResult(formCollectionFake));

            A.CallTo(() => formCollectionFake.Get("client_id")).Returns(null);

            var parser = new VaultPostBodySecretParser(secretStoreFake, parserFake);

            var parseResult = parser.ParseAsync(new Dictionary<string, object>()).Result;

            parseResult.Should().BeNull();
        }

        [Fact]
        public void ParseAsync_ReturnNullIfClientSecretNull()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();
            var formCollectionFake = A.Fake<IFormCollection>();

            A.CallTo(() => parserFake.ReadRequestFormAsync(A<IDictionary<string, object>>._))
             .Returns(Task.FromResult(formCollectionFake));

            A.CallTo(() => formCollectionFake.Get("client_id")).Returns("client123");
            A.CallTo(() => formCollectionFake.Get("client_secret")).Returns(null);

            var parser = new VaultPostBodySecretParser(secretStoreFake, parserFake);

            var parseResult = parser.ParseAsync(new Dictionary<string, object>()).Result;

            parseResult.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAsync_ReturnsNullIfDecryptKeyReturnsNull(string decryptedSecret)
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();
            var formCollectionFake = A.Fake<IFormCollection>();

            A.CallTo(() => parserFake.ReadRequestFormAsync(A<IDictionary<string, object>>._))
             .Returns(Task.FromResult(formCollectionFake));

            A.CallTo(() => formCollectionFake.Get("client_id")).Returns("client123");
            A.CallTo(() => formCollectionFake.Get("client_secret")).Returns("clientsecret123");

            A.CallTo(() => secretStoreFake.GetDecryptedSecret("client123", "clientsecret123"))
             .Returns(Task.FromResult(decryptedSecret));

            var parser = new VaultPostBodySecretParser(secretStoreFake, parserFake);

            var parseResult = parser.ParseAsync(new Dictionary<string, object>()).Result;

            parseResult.Should().BeNull();
        }

        [Fact]
        public void ParseAsync_ReturnsParsedSecret()
        {
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var parserFake = A.Fake<IRequestParser>();
            var formCollectionFake = A.Fake<IFormCollection>();

            A.CallTo(() => parserFake.ReadRequestFormAsync(A<IDictionary<string, object>>._))
             .Returns(Task.FromResult(formCollectionFake));

            A.CallTo(() => formCollectionFake.Get("client_id")).Returns("client123");
            A.CallTo(() => formCollectionFake.Get("client_secret")).Returns("clientsecret123");

            A.CallTo(() => secretStoreFake.GetDecryptedSecret("client123", "clientsecret123"))
             .Returns(Task.FromResult("decryptedsecret123"));

            var parser = new VaultPostBodySecretParser(secretStoreFake, parserFake);

            var parseResult = parser.ParseAsync(new Dictionary<string, object>()).Result;

            parseResult.Id.Should().Be("client123");
            parseResult.Credential.Should().Be("decryptedsecret123");
            parseResult.Type.Should().Be(Constants.VaultSharedSecretType);
        }
    }
}
