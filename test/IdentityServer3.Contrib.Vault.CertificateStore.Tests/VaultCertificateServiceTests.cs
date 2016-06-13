// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Tests
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using IdentityServer3.Core.Configuration;
    using Interfaces;
    using Xunit;

    public class VaultCertificateServiceTests
    {
        [Fact]
        public void Ctor_IdentityServerOptionsNull_ThrowsArgumentNullException()
        {
            // Act
            Action ctor = () => new VaultCertificateService(null, null, null, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [Fact]
        public void Ctor_VaultClientNull_ThrowsArgumentNullException()
        {
            // Act
            Action ctor = () => new VaultCertificateService(new IdentityServerOptions(), null, null, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("vaultClient");
        }

        [Fact]
        public void Ctor_CertificateHelperNull_ThrowsArgumentNullException()
        {
            // Arrange
            var vaultCertificateStoreFake = A.Fake<IVaultCertificateStore>();

            // Act
            Action ctor = () => new VaultCertificateService(new IdentityServerOptions(), vaultCertificateStoreFake, null, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("certificateHelper");
        }

        [Fact]
        public void Ctor_ProviderHelperNull_ThrowsArgumentNullException()
        {
            // Arrange
            var vaultCertificateStoreFake = A.Fake<IVaultCertificateStore>();
            var certificateHelperFake = A.Fake<IX509Certificate2Helper>();

            // Act
            Action ctor = () => new VaultCertificateService(new IdentityServerOptions(), vaultCertificateStoreFake, certificateHelperFake, null);

            // Assert
            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("cryptoServiceProviderHelper");
        }
    }
}
