namespace ServiceStack.Vault.SecretStore.ClientDemo
{
    using System.IO;
    using Authentication.IdentityServer;
    using Authentication.IdentityServer.Enums;
    using Authentication.IdentityServer.Vault;
    using Configuration;
    using Funq;
    using Razor;

    class AppHost : AppSelfHostBase
    {
        private readonly string serviceUrl;

        public AppHost(string serviceUrl, IAppSettings appSettings)
            : base(appSettings.GetString("ServiceName"), typeof(AppHost).Assembly)
        {
            this.serviceUrl = serviceUrl;
            this.AppSettings = appSettings;
        }

        public override void Configure(Container container)
        {
            this.Plugins.Add(new RazorFormat());
            SetConfig(new HostConfig
            {
#if DEBUG
                DebugMode = true,
                WebHostPhysicalPath = Path.GetFullPath(Path.Combine("~".MapServerPath(), "..", "..")),
#endif
                WebHostUrl = serviceUrl
            });

            Plugins.Add(new IdentityServerAuthFeature
            {
                AuthProviderType = IdentityServerAuthProviderType.UserAuthProvider,
                AuthRealm = "http://localhost:5000/",
                ClientId = AppSettings.GetString("ServiceName"),
                Scopes = "openid profile service1 email offline_access"
            });

            Plugins.Add(new IdentityServerVaultAuthFeature
            {
                VaultAppRoleId = AppSettings.GetString("AppRoleId"),
                VaultAppRoleSecretId = AppSettings.GetString("AppSecretId"),

                VaultEncryptionKey = AppSettings.GetString("ServiceName")
            });
        }
    }
}
