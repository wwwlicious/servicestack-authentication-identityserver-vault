// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault
{
    using System.Security.Cryptography.X509Certificates;
    using IdentityServer;
    using Interfaces;
    using ServiceStack.Vault.Core;
    using ServiceStack.Vault.Core.VaultAuth;

    public class IdentityServerVaultAuthFeature : IdentityServerAuthFeature
    {
        public const string VaultUriAppSetting = "vault.uri";
        public const string VaultAppIdAppSetting = "vault.app-id";
        public const string VaultUserIdAppSetting = "vault.user-id";
        public const string VaultEncryptionKeyAppSetting = "vault.encryption.key";
                    
        public const string DefaultVaultUri = "http://127.0.0.1:8200";

        public X509Certificate2 VaultCertificate { get; set; }

        public override void Register(IAppHost appHost)
        {
            if (string.IsNullOrEmpty(appHost.AppSettings.GetString(VaultUriAppSetting)))
            {
                appHost.AppSettings.Set(VaultUriAppSetting, DefaultVaultUri);
            }

            appHost.AppSettings.GetString(VaultAppIdAppSetting).ThrowIfNullOrEmpty("IAppSettings - vault.app-id not set");
            appHost.AppSettings.GetString(VaultUserIdAppSetting).ThrowIfNullOrEmpty("IAppSettings - vault.user-id not set");            

            appHost.AppSettings.Set(VaultEncryptionKeyAppSetting, HostContext.ServiceName);

            var vaultAuth = new VaultAppIdAuth(
                appHost.AppSettings.GetString(VaultAppIdAppSetting),
                appHost.AppSettings.GetString(VaultUserIdAppSetting)
            );
            var vaultClient = new VaultClient(vaultAuth, appHost.AppSettings.GetString(VaultUriAppSetting), VaultCertificate);

            appHost.Register<IClientSecretStore>(new VaultClientSecretStore(appHost.AppSettings, vaultClient));

            base.Register(appHost);
        }
    }
}
