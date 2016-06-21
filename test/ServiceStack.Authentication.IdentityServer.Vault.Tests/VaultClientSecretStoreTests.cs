// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Auth.IdentityServer.Vault.Tests
{
    using System;
    using Authentication.IdentityServer.Vault;
    using Configuration;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class VaultClientSecretStoreTests
    {
        [Fact]
        public void Ctor_AppSettingsNull_ThrowsException()
        {
            Action ctor = () => new VaultClientSecretStore(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("appSettings");
        }

        [Fact]
        public void Ctor_VaultClientNull_ThrowsException()
        {
            var appSettingsFake = A.Fake<IAppSettings>();

            Action ctor = () => new VaultClientSecretStore(appSettingsFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultClient");
        }
    }
}
