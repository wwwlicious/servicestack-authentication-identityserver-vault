// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault.Tests
{
    using System;
    using Vault;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Xunit;

    public class VaultClientSecretStoreTests
    {
        [Fact]
        public void Ctor_AppSettingsNull_ThrowsException()
        {
            Action ctor = () => new VaultClientSecretStore(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("settings");
        }

        [Fact]
        public void Ctor_VaultClientNull_ThrowsException()
        {
            var appSettingsFake = A.Fake<IIdentityServerVaultAuthSettings>();

            Action ctor = () => new VaultClientSecretStore(appSettingsFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultClient");
        }
    }
}
