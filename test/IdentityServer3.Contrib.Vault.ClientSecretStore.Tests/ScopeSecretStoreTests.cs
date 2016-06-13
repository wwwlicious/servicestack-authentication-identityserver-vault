// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Models;
    using Core.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Xunit;

    public class ScopeSecretStoreTests
    {
        [Fact]
        public void Ctor_VaultClientNull_ThrowsArgumentNullException()
        {
            // Act
            Action ctor = () => new ScopeSecretStore(null, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("secretStore");
        }

        [Fact]
        public void Ctor_ScopeStoreNull_ThrowsArgumentNullException()
        {
            // Arrange
            var vaultSecretStore = A.Fake<IVaultSecretStore>();

            // Act
            Action ctor = () => new ScopeSecretStore(vaultSecretStore, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("scopeStore");
        }

        [Fact]
        public void FindScopesAsync_NullScopes_ReturnsEmpty()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(null).Result;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindScopesAsync_EmptyScopes_ReturnsEmpty()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(new List<string>()).Result;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindScopesAsync_NullScopeResults_ReturnsEmpty()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            A.CallTo(() => scopeStoreFake.FindScopesAsync(A<IEnumerable<string>>._))
             .Returns(Task.FromResult((IEnumerable<Scope>)null));

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(new List<string> { "scope1" }).Result;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindScopesAsync_EmptyScopeResults_ReturnsEmpty()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            A.CallTo(() => scopeStoreFake.FindScopesAsync(A<IEnumerable<string>>._))
             .Returns(Task.FromResult((IEnumerable<Scope>)new List<Scope>()));

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(new List<string> { "scope1" }).Result;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindScopesAsync_StandardScope_SecretStoreNotCalled()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            A.CallTo(() => scopeStoreFake.FindScopesAsync(A<IEnumerable<string>>._))
             .Returns(Task.FromResult((IEnumerable<Scope>)new List<Scope>
                {
                    StandardScopes.Email 
                }));

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(new List<string> {StandardScopes.Email.Name})
                              .Result
                              .ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Name.Should().Be(StandardScopes.Email.Name);
        }

        [Fact]
        public void FindScopesAsync_CustomScope_SecretStoreCalled()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            A.CallTo(() => scopeStoreFake.FindScopesAsync(A<IEnumerable<string>>._))
             .Returns(Task.FromResult((IEnumerable<Scope>)new List<Scope>
                {
                    new Scope { Name = "scope1" }
                }));

            A.CallTo(() => secretStoreFake.GetSecrets("scope1"))
             .Returns(Task.FromResult(new[] {"secret1", "secret2"}));

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.FindScopesAsync(new List<string> { "scope1" })
                              .Result
                              .ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Name.Should().Be("scope1");
            result[0].ScopeSecrets.Count.Should().Be(2);
            result[0].ScopeSecrets[0].Value.Should().Be("secret1");
            result[0].ScopeSecrets[0].Type.Should().Be(Constants.VaultSharedSecretType);
            result[0].ScopeSecrets[1].Value.Should().Be("secret2");
            result[0].ScopeSecrets[1].Type.Should().Be(Constants.VaultSharedSecretType);
        }

        [Fact]
        public void GetScopesAync_CallsUnderlyingScopeStore()
        {
            // Arrange
            var secretStoreFake = A.Fake<IVaultSecretStore>();
            var scopeStoreFake = A.Fake<IScopeStore>();

            A.CallTo(() => scopeStoreFake.GetScopesAsync(true)).Returns(Task.FromResult((IEnumerable<Scope>)new List<Scope>
            {
                new Scope { Name = "scope1" },
                new Scope { Name = "scope2" }
            }));

            var store = new ScopeSecretStore(secretStoreFake, scopeStoreFake);

            // Act
            var result = store.GetScopesAsync().Result.ToList();

            // Assert
            result.Count.Should().Be(2);

            result[0].Name.Should().Be("scope1");
            result[1].Name.Should().Be("scope2");
        }
    }
}
