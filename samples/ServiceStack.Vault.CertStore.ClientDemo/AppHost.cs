namespace ServiceStack.Vault.CertStore.ClientDemo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Authentication.IdentityServer;
    using Authentication.IdentityServer.Enums;
    using Authentication.IdentityServer.Extensions;
    using Funq;
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
