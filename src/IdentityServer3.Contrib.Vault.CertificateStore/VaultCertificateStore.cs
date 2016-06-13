// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore
{
    using System;
    using Core.Helpers;
    using Core.Interfaces;
    using DTO;
    using IdentityServer3.Core.Logging;
    using Interfaces;

    public class VaultCertificateStore : IVaultCertificateStore
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IVaultClient vaultClient;

        private readonly string roleName;
        private readonly string commonName;

        public VaultCertificateStore(IVaultClient vaultClient, string roleName, string commonName)
        {
            this.vaultClient = vaultClient.ThrowIfNull(nameof(vaultClient));
            this.roleName = roleName.ThrowIfNullOrEmpty(nameof(roleName));
            this.commonName = commonName.ThrowIfNullOrEmpty(nameof(commonName));
        }

        public VaultCertificate GetCertificate()
        {
            try
            {
                using (var client = vaultClient.ServiceClient)
                {
                    var request = new PkiIssue(commonName);
                    var response = client.Post<PkiIssue, PkiIssueResult>($"v1/pki/issue/{roleName}", request);
                    if (response?.Data == null) return null;
                    return new VaultCertificate
                    {
                        Certificate = response.Data.Certificate,
                        PrivateKey = response.Data.PrivateKey
                    };
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, () => $"Unable to get certificate role: {roleName} cn: {commonName}", exception);
                return null;
            }
        }
    }
}
