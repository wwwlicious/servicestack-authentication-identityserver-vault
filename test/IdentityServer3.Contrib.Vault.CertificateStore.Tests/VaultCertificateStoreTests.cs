// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Tests
{
    using System;
    using System.Net;
    using System.Text;
    using Core.Interfaces;
    using DTO;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class VaultCertificateStoreTests
    {
        [Fact]
        public void Ctor_VaultOptionsNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new VaultCertificateStore(null, null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultClient");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Ctor_VaultOptionsRoleNameNullOrEmpty_ThrowsArgumentException(string roleName)
        {
            var clientFake = A.Fake<IVaultClient>();

            Action ctor = () => new VaultCertificateStore(clientFake, roleName, null);

            ctor.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("roleName");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Ctor_VaultOptionsCommonNameNullOrEmpty_ThrowsArgumentException(string commonName)
        {
            var clientFake = A.Fake<IVaultClient>();

            Action ctor = () => new VaultCertificateStore(clientFake, "roleName", commonName);

            ctor.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("commonName");
        }

        [Fact]
        public void GetCertificate_ServiceClientThrowsException_ReturnsNull()
        {
            // Arrange
            var clientFake = A.Fake<IVaultClient>();
            A.CallTo(() => clientFake.ServiceClient).Throws<WebException>();

            var store = new VaultCertificateStore(clientFake, "roleName", "commonName");                

            // Act
            var result = store.GetCertificate();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetCertificate_PostThrowsException_ReturnsNull()
        {
            // Arrange
            var clientFake = A.Fake<IVaultClient>();
            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => clientFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>._))
             .Throws<WebException>();

            var store = new VaultCertificateStore(clientFake, "roleName", "commonName");

            // Act
            var result = store.GetCertificate();

            // Assert
            result.Should().BeNull();
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>.That.Matches(r => r.CommonName == "commonName")))
             .MustHaveHappened();
        }

        [Fact]
        public void GetCertificate_PostReturnsNull_ReturnsNull()
        {
            // Arrange
            var clientFake = A.Fake<IVaultClient>();
            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => clientFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>._))
             .Returns(null);

            var store = new VaultCertificateStore(clientFake, "roleName", "commonName");

            // Act
            var result = store.GetCertificate();

            // Assert
            result.Should().BeNull();
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>.That.Matches(r => r.CommonName == "commonName")));
        }

        [Fact]
        public void GetCertificate_PostReturnsNullData_ReturnsNull()
        {
            // Arrange
            var clientFake = A.Fake<IVaultClient>();
            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => clientFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>._))
             .Returns(new PkiIssueResult { Data = null });

            var store = new VaultCertificateStore(clientFake, "roleName", "commonName");

            // Act
            var result = store.GetCertificate();

            // Assert
            result.Should().BeNull();
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>.That.Matches(r => r.CommonName == "commonName")));
        }

        [Fact]
        public void GetCertificate_ReturnsCertificateData()
        {
            // Arrange

            var clientFake = A.Fake<IVaultClient>();
            var httpClientFake = A.Fake<IVaultHttpClient>();
            A.CallTo(() => clientFake.ServiceClient).Returns(httpClientFake);
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>._))
             .Returns(new PkiIssueResult
                {
                    Data = new PkiData
                    {
                        Certificate = Encoding.UTF8.GetBytes("TestCertificate"),
                        PrivateKey = Encoding.UTF8.GetBytes("TestPrivateKey")
                    }
                });

            var store = new VaultCertificateStore(clientFake, "roleName", "commonName");

            // Act
            var result = store.GetCertificate();

            // Assert
            result.Certificate.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("TestCertificate"));
            result.PrivateKey.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("TestPrivateKey"));
            A.CallTo(() => httpClientFake.Post<PkiIssue, PkiIssueResult>("v1/pki/issue/roleName", A<PkiIssue>.That.Matches(r => r.CommonName == "commonName")));
        }
    }
}
