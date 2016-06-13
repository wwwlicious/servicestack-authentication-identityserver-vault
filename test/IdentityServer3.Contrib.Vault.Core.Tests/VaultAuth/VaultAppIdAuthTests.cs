// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core.Tests.VaultAuth
{
    using System;
    using System.Net;
    using Core.VaultAuth;
    using DTO;
    using FakeItEasy;
    using FluentAssertions;
    using Interfaces;
    using Xunit;

    public class VaultAppIdAuthTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void Ctor_AppIdNullOrEmpty_ThrowsArgumentNullException(string appId)
        {
            Action ctor = () => new VaultAppIdAuth(appId, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("appId");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void Ctor_UserIdNullOrEmpty_ThrowsArgumentNullException(string userId)
        {
            Action ctor = () => new VaultAppIdAuth("app1234", userId);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void Authenticate_VaultUriServiceClient_ThrowsException()
        {
            // Arrange
            var vaultClientUriFake = A.Fake<IVaultClientUri>();
            A.CallTo(() => vaultClientUriFake.ServiceClient).Throws<WebException>();

            var auth = new VaultAppIdAuth("app1234", "user1234");

            // Act
            Action authenticate = () => auth.Authenticate(vaultClientUriFake);

            // Assert
            authenticate.ShouldThrow<WebException>();
        }

        [Fact]
        public void Authenticate_Success()
        {
            var vaultClientUriFake = A.Fake<IVaultClientUri>();
            var vaultHttpFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => vaultClientUriFake.ServiceClient).Returns(vaultHttpFake);
            A.CallTo(() => vaultHttpFake.Post<AppIdLogin, AppIdLoginResult>("v1/auth/app-id/login/app1234", A<AppIdLogin>._))
             .Returns(new AppIdLoginResult { Auth = new Auth { ClientToken = "token1234"} });

            var auth = new VaultAppIdAuth("app1234", "user1234");

            // Act
            auth.Authenticate(vaultClientUriFake);

            // Assert
            auth.AuthToken.Should().Be("token1234");

            A.CallTo(() => vaultClientUriFake.ServiceClient)
             .MustHaveHappened();
            A.CallTo(() => vaultHttpFake.Post<AppIdLogin, AppIdLoginResult>("v1/auth/app-id/login/app1234", A<AppIdLogin>.That.Matches(x => x.UserId == "user1234")))
             .MustHaveHappened();
        }       
    }
}
