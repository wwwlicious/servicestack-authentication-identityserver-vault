namespace ServiceStack.Vault.CertStore.ClientDemo
{
    using System.IO;
    using Authentication.IdentityServer;
    using Authentication.IdentityServer.Enums;
    using Funq;
    using Logging;
    using Razor;

    class AppHost : AppSelfHostBase
    {
        private readonly string serviceUrl;

        public AppHost(string serviceUrl)
            : base(Program.ServiceId, typeof(AppHost).Assembly)
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

                ClientId = Program.ServiceId,
                ClientSecret = "F621F470-9731-4A25-80EF-67A6F7C5F4B8",

                Scopes = "openid profile service1 email offline_access"
            });
        }
    }
}
