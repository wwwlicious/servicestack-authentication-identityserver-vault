// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.SecretAppSettings.Tests
{
    using System;
    using Core.Interfaces;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class VaultSecretAppSettingsTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void Get_NameNullOrEmpty_ThrowsArgumentNullException(string name)
        {
            var clientFake = A.Fake<IVaultClient>();

            var appSettings = new VaultSecretAppSettings(clientFake, "path");

            Action get = () => appSettings.Get<string>(name, null);

            get.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("name");
        }

        [Fact]
        public void Get_ClientServiceClientThrowsException_ReturnsDefaultValue()
        {
            var clientFake = A.Fake<IVaultClient>();

            A.CallTo(() => clientFake.ServiceClient).Throws<Exception>();

            var appSettings = new VaultSecretAppSettings(clientFake, "path");
            var result = appSettings.Get("secret", "defaultsecretvalue");

            result.Should().Be("defaultsecretvalue");

            A.CallTo(() => clientFake.ServiceClient).MustHaveHappened();
        }
    }
}
