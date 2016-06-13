// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests
{
    using System;
    using System.Threading.Tasks;
    using Core.Models;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Xunit;

    public class ClientSecretStoreTests
    {
        [Fact]
        public void Ctor_VaultClientNull_ThrowsArgumentNullException()
        {
            // Act
            Action ctor = () => new ClientSecretStore(null, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("secretStore");
        }

        [Fact]
        public void Ctor_ClientStoreNull_ThrowsArgumentNullException()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();

            // Act
            Action ctor = () => new ClientSecretStore(secretStoreFake, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("clientStore");
        }

        [Fact]
        public void FindClientByIdAsync_ClientStoreNull_ReturnsNull()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var clientStoreFake = A.Fake<IClientDataStore>();

            A.CallTo(() => clientStoreFake.FindClientByIdAsync("client1234")).Returns(Task.FromResult<Client>(null));

            var clientSecretStore = new ClientSecretStore(secretStoreFake, clientStoreFake);

            // Act
            var result = clientSecretStore.FindClientByIdAsync("client1234").Result;

            // Assert
            result.Should().BeNull();
            A.CallTo(() => clientStoreFake.FindClientByIdAsync("client1234")).MustHaveHappened();
        }

        [Fact]
        public void FindClientByIdAsync_ClientSecretsNull_ReturnsClientWithEmptyList()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var clientStoreFake = A.Fake<IClientDataStore>();

            A.CallTo(() => clientStoreFake.FindClientByIdAsync("client1234"))
                                          .Returns(Task.FromResult(new Client()));

            A.CallTo(() => secretStoreFake.GetSecrets("client1234"))
                                          .Returns(Task.FromResult<string[]>(null));

            var clientSecretStore = new ClientSecretStore(secretStoreFake, clientStoreFake);

            // Act
            var result = clientSecretStore.FindClientByIdAsync("client1234").Result;

            // Assert
            result.Should().NotBeNull();
            result.ClientSecrets.Should().BeEmpty();
        }

        [Fact]
        public void FindClientByIdAsync_ClientSecretsEmpty_ReturnsClientWithEmptyList()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var clientStoreFake = A.Fake<IClientDataStore>();

            A.CallTo(() => clientStoreFake.FindClientByIdAsync("client1234"))
                                          .Returns(Task.FromResult(new Client()));

            A.CallTo(() => secretStoreFake.GetSecrets("client1234"))
                                          .Returns(Task.FromResult(new string[0]));

            var clientSecretStore = new ClientSecretStore(secretStoreFake, clientStoreFake);

            // Act
            var result = clientSecretStore.FindClientByIdAsync("client1234").Result;

            // Assert
            result.Should().NotBeNull();
            result.ClientSecrets.Should().BeEmpty();
        }

        [Fact]
        public void FindClientByIdAsync_ClientSecretsList_ReturnsClientWithList()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var clientStoreFake = A.Fake<IClientDataStore>();

            A.CallTo(() => clientStoreFake.FindClientByIdAsync("client1234"))
                                          .Returns(Task.FromResult(new Client()));

            A.CallTo(() => secretStoreFake.GetSecrets("client1234"))
                                          .Returns(Task.FromResult(new [] { "secret1", "secret2", "secret3" } ));

            var clientSecretStore = new ClientSecretStore(secretStoreFake, clientStoreFake);

            // Act
            var result = clientSecretStore.FindClientByIdAsync("client1234").Result;

            // Assert
            result.Should().NotBeNull();
            result.ClientSecrets.Should().NotBeEmpty();

            result.ClientSecrets[0].Value.Should().Be("secret1");
            result.ClientSecrets[0].Type.Should().Be(Constants.VaultSharedSecretType);
            result.ClientSecrets[1].Value.Should().Be("secret2");
            result.ClientSecrets[1].Type.Should().Be(Constants.VaultSharedSecretType);
            result.ClientSecrets[2].Value.Should().Be("secret3");
            result.ClientSecrets[2].Type.Should().Be(Constants.VaultSharedSecretType);
        }
    }
}
