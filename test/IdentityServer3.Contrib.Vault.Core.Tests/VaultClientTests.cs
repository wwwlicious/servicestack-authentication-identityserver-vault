// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core.Tests
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Xunit;

    public class VaultClientTests
    {
        [Fact]
        public void Ctor_VaultUriNull_ThrowsException()
        {
            Action ctor = () => new VaultClient(null, null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultUri");
        }

        [Fact]
        public void Ctor_VaultUriVaultAuthNull_ThrowsException()
        {
            Action ctor = () => new VaultClient(null, "http://127.0.0.1:8200", null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultAuth");
        }

        [Fact]
        public void Ctor_VaultUriClientNull_ThrowsException()
        {
            Action ctor = () => new VaultClient((IVaultClientUri)null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultClientUri");
        }

        [Fact]
        public void Ctor_VaultUriClientVaultAuthNull_ThrowsException()
        {
            var httpClientUriFake = A.Fake<IVaultClientUri>();

            Action ctor = () => new VaultClient(httpClientUriFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultAuth");
        }

        [Fact]
        public void ServiceClient_AuthTokenNotSet_Authenticate()
        {
            // Arrange
            var vaultClientUriFake = A.Fake<IVaultClientUri>();

            string authToken = null;
            var httpAuthFake = A.Fake<IVaultAuth>();
            A.CallTo(() => httpAuthFake.AuthToken).Returns(authToken);
            A.CallTo(() => httpAuthFake.Authenticate(vaultClientUriFake))
             .Invokes(() => authToken = "AUTH12345");

            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => vaultClientUriFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.AddHeader("X-Vault-Token", "AUTH12345"));

            var client = new VaultClient(vaultClientUriFake, httpAuthFake);

            // Act
            var serviceClient = client.ServiceClient;

            // Assert
            serviceClient.Should().Be(httpClientFake);

            A.CallTo(() => httpAuthFake.AuthToken).MustHaveHappened();
            A.CallTo(() => httpAuthFake.Authenticate(vaultClientUriFake)).MustHaveHappened();
            A.CallTo(() => vaultClientUriFake.ServiceClient).MustHaveHappened();
            A.CallTo(() => httpClientFake.AddHeader("X-Vault-Token", "AUTH12345"));
        }

        [Fact]
        public void ServiceClient_AuthTokenSet_Authenticate()
        {
            // Arrange
            var vaultClientUriFake = A.Fake<IVaultClientUri>();

            var httpAuthFake = A.Fake<IVaultAuth>();
            A.CallTo(() => httpAuthFake.AuthToken).Returns("AUTH12345");

            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => vaultClientUriFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.AddHeader("X-Vault-Token", "AUTH12345"));

            var client = new VaultClient(vaultClientUriFake, httpAuthFake);

            // Act
            var serviceClient = client.ServiceClient;

            // Assert
            serviceClient.Should().Be(httpClientFake);

            A.CallTo(() => httpAuthFake.AuthToken).MustHaveHappened();
            A.CallTo(() => vaultClientUriFake.ServiceClient).MustHaveHappened();
            A.CallTo(() => httpClientFake.AddHeader("X-Vault-Token", "AUTH12345"));
        }
    }
}
