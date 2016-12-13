// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Demo
{
    using System.IO;
    using Funq;
    using global::ServiceStack;
    using global::ServiceStack.Authentication.IdentityServer.Extensions;
    using global::ServiceStack.Razor;
    using ServiceStack.Authentication.IdentityServer;
    using ServiceStack.Authentication.IdentityServer.Vault;

    class AppHost : AppSelfHostBase
    {
        public static string ServiceId = "service1";
        public static string ServiceAppId = "f8a5a40f-ecd9-43da-a009-82f180e1ef84";
        public static string ServiceUserId = "27ded1df-7aca-40ba-a825-cc9bf5cb7f88";

        private readonly string serviceUrl;        

        public AppHost(string serviceUrl)
            : base(ServiceId, typeof (AppHost).Assembly)
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

            AppSettings.SetUserAuthProvider()
                       .SetAuthRealm("http://localhost:5000/")
                       .SetClientId(ServiceId)
                       .SetScopes("openid profile service1 email offline_access");

            Plugins.Add(new IdentityServerAuthFeature
            {
                
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
