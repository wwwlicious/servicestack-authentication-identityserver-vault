// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.CertificateStore.Options
{
    using System;

    [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
    public class VaultCertificateStoreAppIdOptions : VaultCertificateStoreOptions
    {
        public string AppId { get; set; }

        public string UserId { get; set; }
    }
}
