namespace ServiceStack.Vault.SecretStore.ClientDemo
{
    using System.IO;
    using Authentication.IdentityServer;
    using Authentication.IdentityServer.Enums;
    using Authentication.IdentityServer.Vault;
    using Funq;
    using Logging;
    using Razor;

    class AppHost : AppSelfHostBase
    {
        public static string ServiceId = "service1";
        public static string ServiceAppId = "f8a5a40f-ecd9-43da-a009-82f180e1ef84";
        public static string ServiceUserId = "27ded1df-7aca-40ba-a825-cc9bf5cb7f88";

        private readonly string serviceUrl;

        public AppHost(string serviceUrl)
            : base(ServiceId, typeof(AppHost).Assembly)
        {
            this.serviceUrl = serviceUrl;
        }

        public override void Configure(Container container)
        {
            LogManager.LogFactory = new ConsoleLogFactory(debugEnabled: true);

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
                ClientId = ServiceId,
                Scopes = "openid profile service1 email offline_access"
            });

            Plugins.Add(new IdentityServerVaultAuthFeature
            {
                VaultAppId = ServiceAppId,
                VaultUserId = ServiceUserId,
                VaultEncryptionKey = ServiceId
            });
        }
    }
}
