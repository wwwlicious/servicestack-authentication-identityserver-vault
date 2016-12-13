// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault
{
    using System.Security.Cryptography.X509Certificates;
    using Configuration;
    using IdentityServer.Interfaces;
    using Interfaces;
    using ServiceStack.Vault.Core;
    using ServiceStack.Vault.Core.VaultAuth;

    public class IdentityServerVaultAuthFeature : IPlugin, IIdentityServerVaultAuthSettings
    {
        protected readonly IAppSettings AppSettings;

        public const string DefaultVaultUri = "http://127.0.0.1:8200";

        public X509Certificate2 VaultCertificate { get; set; }

        public string VaultUri
        {
            get { return AppSettings.Get(ConfigKeys.VaultUri, DefaultVaultUri); }
            set { AppSettings.Set(ConfigKeys.VaultUri, value); }
        }

        public string VaultAppId
        {
            get { return AppSettings.Get(ConfigKeys.VaultAppId, string.Empty); }
            set { AppSettings.Set(ConfigKeys.VaultAppId, value); }
        }

        public string VaultUserId
        {
            get { return AppSettings.Get(ConfigKeys.VaultUserId, string.Empty); }
            set { AppSettings.Set(ConfigKeys.VaultUserId, value); }
        }

        public string VaultEncryptionKey
        {
            get { return AppSettings.Get(ConfigKeys.VaultEncryptionKey, HostContext.ServiceName); }
            set { AppSettings.Set(ConfigKeys.VaultEncryptionKey, value); }
        }

        public IdentityServerVaultAuthFeature(IAppSettings appSettings = null)
        {
            this.AppSettings = appSettings ?? ServiceStackHost.Instance.AppSettings;
        }

        public void Register(IAppHost appHost)
        {
            VaultUri.ThrowIfNullOrEmpty(nameof(VaultUri));
            VaultAppId.ThrowIfNullOrEmpty("IAppSettings - vault.app-id not set");
            VaultUserId.ThrowIfNullOrEmpty("IAppSettings - vault.user-id not set");      

            var vaultAuth = new VaultAppIdAuth(VaultAppId, VaultUserId);
            var vaultClient = new VaultClient(vaultAuth, VaultUri, VaultCertificate);

            appHost.Register<IClientSecretStore>(new VaultClientSecretStore(this, vaultClient));
        }
    }
}
