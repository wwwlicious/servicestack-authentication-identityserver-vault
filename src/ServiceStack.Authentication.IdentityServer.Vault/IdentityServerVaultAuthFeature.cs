namespace ServiceStack.Authentication.IdentityServer.Vault
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Configuration;
    using IdentityServer.Interfaces;
    using Interfaces;
    using ServiceStack.Vault.Core;
    using ServiceStack.Vault.Core.Enums;
    using ServiceStack.Vault.Core.Interfaces;
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

        public VaultAuthMethod VaultAuthMethod
        {
            get { return AppSettings.Get(ConfigKeys.VaultAuthMethod, VaultAuthMethod.AppRole); }
            set { AppSettings.Set(ConfigKeys.VaultAuthMethod, value); }
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
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

        public string VaultUserPassword
        {
            get { return AppSettings.Get(ConfigKeys.VaultUserPassword, string.Empty); }
            set { AppSettings.Set(ConfigKeys.VaultUserPassword, value); }
        }

        public string VaultAppRoleId
        {
            get { return AppSettings.Get(ConfigKeys.VaultAppRoleId, string.Empty); }
            set { AppSettings.Set(ConfigKeys.VaultAppRoleId, value); }
        }

        public string VaultAppRoleSecretId
        {
            get { return AppSettings.Get(ConfigKeys.VaultAppRoleSecretId, string.Empty); }
            set { AppSettings.Set(ConfigKeys.VaultAppRoleSecretId, value); }
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

            var vaultAuth = GetVaultAuth();
            var vaultClient = new VaultClient(vaultAuth, VaultUri, VaultCertificate);

            appHost.Register<IClientSecretStore>(new VaultClientSecretStore(this, vaultClient));
        }

        private IVaultAuth GetVaultAuth()
        {
            switch (VaultAuthMethod)
            {
#pragma warning disable 618
                case VaultAuthMethod.AppId:
                    return VaultAppIdAuth();
#pragma warning restore 618                
                case VaultAuthMethod.User:
                    return VaultUserPassAuth();
                default:
                    return VaultAppRoleAuth();
            }
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        private VaultAppIdAuth VaultAppIdAuth()
        {
            VaultAppId.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultAppId} not set");
            VaultUserId.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultUserId} not set");
            return new VaultAppIdAuth(VaultAppId, VaultUserId);
        }

        private VaultAppRoleAuth VaultAppRoleAuth()
        {
            VaultAppRoleId.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultAppRoleId} not set");
            VaultAppRoleSecretId.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultAppRoleSecretId} not set");

            return new VaultAppRoleAuth(VaultAppRoleId, VaultAppRoleSecretId);
        }

        private VaultUserPassAuth VaultUserPassAuth()
        {
            VaultUserId.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultUserId} not set");
            VaultUserPassword.ThrowIfNullOrEmpty($"IAppSettings - {ConfigKeys.VaultUserPassword} not set");

            return new VaultUserPassAuth(VaultUserId, VaultUserPassword);
        }
    }
}
